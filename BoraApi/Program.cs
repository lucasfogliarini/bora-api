using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.OData;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Authentication;
using BoraApi.OData;
using BoraApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Bora.Repository;

const string VERSION = "1.0.0";
const string APP_NAME = "BoraApi";

var applicationBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = AddServices(applicationBuilder).Build();

app.Services.Migrate();

await app.SeedAsync();

Run(app);//Runs an application and block the calling thread until host shutdown.

static WebApplicationBuilder AddServices(WebApplicationBuilder builder)
{
	var mvcBuilder = builder.Services.AddControllers();
	mvcBuilder.AddNewtonsoftJson(options =>
	   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
	);
	mvcBuilder.AddOData(opt =>
	{
		opt.EnableQueryFeatures(50);
		opt.AddRouteComponentsUsingODataControllers();
	});

	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen((o) =>
	{
        o.OperationFilter<ODataOperationFilter>();
		o.AddSwaggerJwtAuthentication();
    });

	Console.WriteLine($"Starting {APP_NAME} version: {VERSION}");
	Console.WriteLine();

	builder.Services.AddLogging((loggingBuilder) =>
	{
		loggingBuilder.AddDebug();
	});

	builder.AddServices();
    builder.AddRepository();
    //builder.Services.AddSpotifyService();
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<BoraDbContext>(customTestQuery: async (context, ct) =>
        {
            if (context.Database.ProviderName.Contains("InMemory"))
                return false;
            return await context.Database.CanConnectAsync(ct);
        });

    builder.Services.AddProblemDetails(x =>
	{
		x.MapToStatusCode<ValidationException>(StatusCodes.Status400BadRequest);
		x.MapToStatusCode<AuthenticationException>(StatusCodes.Status401Unauthorized);
		x.IncludeExceptionDetails = (ctx, ex) =>
		{
			return true;
		};
	});

	return builder;
}

static void Run(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(x => x
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowAnyOrigin());

	app.UseHttpsRedirection();

	app.UseForwardedHeaders(new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedProto
	});

	app.UseProblemDetails();

	app.UseAuthentication();//must be before UseAuthorization
	app.UseAuthorization();

	app.MapControllers();
	app.MapGet("/version", async context =>
	{
		await context.Response.WriteAsync(VERSION);
	});

    UseHealthChecks(app);

    app.MapGet("/loaderio-bb899ee09dc2c7596f6f3333be0b05af.txt", async context =>
	{
		string loaderVerificationToken = "loaderio-bb899ee09dc2c7596f6f3333be0b05af";
		await context.Response.WriteAsync(loaderVerificationToken);
	});

	app.Run();
}

static void UseHealthChecks(WebApplication app)
{
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true, // Executa todos os health checks
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    data = entry.Value.Data
                })
            });

            await context.Response.WriteAsync(result);
        }
    });
}
