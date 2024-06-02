using Bora.Events;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2.Responses;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpOverrides;
using Bora.Accounts;
using System.Security.Authentication;
using Bora.Entities;
using Bora;
using BoraApi.OData;
using BoraApi.Jwt;

const string VERSION = "1.0.0";
const string APP_NAME = "BoraApi";

var applicationBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = AddServices(applicationBuilder).Build();

app.Services.Migrate();

await SeedAsync(app);

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
		o.AddSwaggerAuthentication();
    });

	Console.WriteLine($"Starting {APP_NAME} version: {VERSION}");
	Console.WriteLine();

	builder.Services.AddCors();
	builder.Services.AddLogging((loggingBuilder) =>
	{
		loggingBuilder.AddDebug();
	});

	builder.AddJwtAuthentication();

	AddGoogleCalendar(builder);

	AddRepository(builder);
    builder.Services.AddServices();
	//builder.Services.AddSpotifyService();

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

	app.MapGet("/loaderio-bb899ee09dc2c7596f6f3333be0b05af.txt", async context =>
	{
		string loaderVerificationToken = "loaderio-bb899ee09dc2c7596f6f3333be0b05af";
		await context.Response.WriteAsync(loaderVerificationToken);
	});

	app.Run();
}

static void AddRepository(WebApplicationBuilder builder)
{
	try
	{
        var repositoryConnectionString = TryGetConnectionString(builder);
        builder.Services.AddEFCoreRepository(EFCoreProvider.SqlServer, repositoryConnectionString);
        //builder.Services.AddDapperRepository(repositoryConnectionString);
    }
    catch (Exception)
	{
        builder.Services.AddEFCoreRepository(EFCoreProvider.InMemory);
    }
}

static void AddGoogleCalendar(WebApplicationBuilder builder)
{
	var googleCalendarSection = builder.Configuration.GetSection(GoogleCalendarConfiguration.AppSettingsKey);
	var googleCalendarConfig = googleCalendarSection.Get<GoogleCalendarConfiguration>();
	builder.Services.AddSingleton(googleCalendarConfig);

	//https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-asp.net-core-3
	builder.Services
		.AddAuthentication(o =>
		{
			// This forces challenge results to be handled by Google OpenID Handler, so there's no
			// need to add an AccountController that emits challenges for Login.
			o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
			// This forces forbid results to be handled by Google OpenID Handler, which checks if
			// extra scopes are required and does automatic incremental auth.
			o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;

			o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		})
		.AddCookie()
		.AddGoogleOpenIdConnect(options =>
		{
			options.ClientId = googleCalendarConfig.ClientId;
			options.ClientSecret = googleCalendarConfig.ClientSecret;
			options.Events.OnTokenValidated += async (ctx) =>
			{
				var accountDataStore = builder.Services.BuildServiceProvider().GetService<IAccountDataStore>();
				TokenResponse tokenResponse = new()
				{
					IssuedUtc = DateTime.UtcNow,
					AccessToken = ctx.TokenEndpointResponse.AccessToken,
					RefreshToken = ctx.TokenEndpointResponse.RefreshToken,
					ExpiresInSeconds = long.Parse(ctx.TokenEndpointResponse.ExpiresIn),//TimeSpan.FromDays(300).Seconds,
				};
				string email = ctx.Principal.FindFirst(ClaimTypes.Email).Value;
				await accountDataStore.AuthorizeCalendarAsync(email, tokenResponse);
			};
		});
}

static async Task SeedAsync(WebApplication app)
{
	using var scope = app.Services.CreateScope();
	var databaseRepository = scope.ServiceProvider.GetService<IRepository>();
	await databaseRepository!.SeedAsync();
}

static string? TryGetConnectionString(WebApplicationBuilder builder)
{
	var boraRepositoryConnectionStringKey = "ConnectionStrings:BoraRepository";
	Console.WriteLine($"Trying to get a database connectionString '{boraRepositoryConnectionStringKey}' from Configuration.");
	var connectionString = builder.Configuration[boraRepositoryConnectionStringKey];
    if (connectionString == null)
		throw new Exception($"{boraRepositoryConnectionStringKey} was not found! From builder.Configuration[{boraRepositoryConnectionStringKey}]");

    Console.WriteLine();
    return connectionString;
}
