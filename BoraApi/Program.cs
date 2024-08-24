using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.OData;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Authentication;
using BoraApi.OData;
using BoraApi;

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

	builder.Services.AddCors();
	builder.Services.AddLogging((loggingBuilder) =>
	{
		loggingBuilder.AddDebug();
	});

	builder.AddAuthentications();
    builder.AddRepository();
    builder.Services.AddServices();
	//builder.Services.AddSpotifyService();
	builder.Services.AddHealthChecks();

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

    app.MapHealthChecks("/health");

    app.MapGet("/loaderio-bb899ee09dc2c7596f6f3333be0b05af.txt", async context =>
	{
		string loaderVerificationToken = "loaderio-bb899ee09dc2c7596f6f3333be0b05af";
		await context.Response.WriteAsync(loaderVerificationToken);
	});

	app.Run();
}
