using Bora.Api;
using Bora.Events;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2.Responses;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpOverrides;
using Bora.Accounts;
using System.Security.Authentication;
using Spotify;
using Bora.Entities;
using SpotifyApi.NetCore;

const string VERSION = "1.0.0";
const string APP_NAME = "BoraApi";

var applicationBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = AddServices(applicationBuilder).Build();

MigrateAndSeed(app);

Run(app);

static WebApplicationBuilder AddServices(WebApplicationBuilder builder)
{
    var mvcBuilder = builder.Services.AddControllers();
    mvcBuilder.AddNewtonsoftJson(options =>
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );
    mvcBuilder.AddOData(opt =>
    {
        opt.EnableQueryFeatures(50);
        opt.AddRouteComponentsODataControllers();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(APP_NAME, new OpenApiInfo { Title = "Bora.Api", Version = VERSION });
    });
    Console.WriteLine($"Starting {APP_NAME} version: {VERSION}");
	Console.WriteLine();

	builder.Services.AddCors();
    builder.Services.AddLogging((loggingBuilder) =>
    {
        loggingBuilder.AddDebug();
    });

    Jwt.AddJwtAuthentication(builder.Services, builder.Configuration);

    AddGoogleCalendar(builder);

    var storageConnectionString = TryGetConnectionString(builder);

	builder.Services.AddBoraAzureTablesRepository(storageConnectionString);
    builder.Services.AddServices();
    builder.Services.AddSpotifyService();

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
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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

    app.Run();
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

static async void MigrateAndSeed(WebApplication app)
{
    //using var scope = app.Services.CreateScope();
	//var boraDbContext = scope.ServiceProvider.GetService<BoraDbContext>();
	//Bora.Repository.MSSQL.Seed.MigrateAndSeed(boraDbContext);
}

static string? TryGetConnectionString(WebApplicationBuilder builder)
{
	var connectionStringName = "boraRepository";
	Console.WriteLine($"Trying to get a database connectionString '{connectionStringName}' from appsettings");
	var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
	if (connectionString == null)
		Console.WriteLine("ConnectionString was not found.");
	else
	{
		Console.WriteLine($"ConnectionString was found.");
	}
    return connectionString;
}
