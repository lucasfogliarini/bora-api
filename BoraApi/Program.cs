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
using Bora;

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

	var repositoryConnectionString = TryGetConnectionString(builder);

	builder.Services.AddDapperRepository(repositoryConnectionString);
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

	app.MapGet("/loaderio-bb899ee09dc2c7596f6f3333be0b05af.txt", async context =>
	{
		string loaderVerificationToken = "loaderio-bb899ee09dc2c7596f6f3333be0b05af";
		await context.Response.WriteAsync(loaderVerificationToken);
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

static async Task SeedAsync(WebApplication app)
{
	using var scope = app.Services.CreateScope();
	var databaseRepository = scope.ServiceProvider.GetService<IRepository>();
	var hasAccount = databaseRepository.Any<Account>();
	if (!hasAccount)
	{
		var accounts = new List<Account>
		{
			new("lucasfogliarini@gmail.com", 1, new DateTime(2024, 01, 01, 23, 47, 1))
			{
				Name = "Lucas Fogliarini Pedroso",
				Photo = "https://lh3.googleusercontent.com/a-/AOh14Ggingx4m5A-dFGLwEJv-acJ-KEDtApHCAO0NxfUig=s96-c",
				WhatsApp = "51992364249",
				Instagram = "lucasfogliarini",
				Spotify = "12145833562",
				Linkedin = "lucasfogliarini"
			},
			new("luanaleticiabueno@gmail.com", 2, new DateTime(2024, 01, 01, 23, 47, 2))
			{
				Name = "Luana Bueno",
				WhatsApp = "5193840006",
				Instagram = "luanabuenoflores",
				Spotify = "224juavirzfsjsxt5yva6fvly",
				Photo= "https://lh3.googleusercontent.com/a-/AOh14GhWN-zhlu_93Me88oT9v8554pdaJQdNYKpUp-i__c0=s340-p-k-rw-no",
			},
			new("gui_staub@hotmail.com", 3, new DateTime(2024, 01, 01, 23, 47, 3))
			{
				Name = "Guilherme Staub",
				Photo= "https://lh3.googleusercontent.com/a-/AOh14Gi14cQFSeyn5q6u3ZB_derhI7yIcA9dgX27OkBl=s96-c",
			},
			new("varreira.adv@gmail.com", 4, new DateTime(2024, 01, 01, 23, 47, 4))
			{
				Name = "Anderson Varreira",
				Photo= "https://lh3.googleusercontent.com/a/AATXAJw-6J_C5vAh-d9Gp3ssN_ziJrOkzp6HMWXE6Ubm=s96-c",
			},
			new("lucasbuenomagalhaes@gmail.com", 5, new DateTime(2024, 01, 01, 23, 47, 5))
			{
				Name = "Lucas Bueno",
				Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggfyxso7uuqWxLMqvI3JTDOcKDRKkOgsz0oOwLWPw=s96-c",
			}
		};
		foreach (var account in accounts)
		{
			account.Id = null;
			databaseRepository.Add(account);
		}

		await databaseRepository.CommitAsync();
	}


	//var homeContents = new List<Content>
	//{
	//	new() {
	//		Collection = "home",
	//		Key = "boraLink",
	//		Text = "/lucasfogliarini"
	//	},
	//	new() {
	//		Collection = "home",
	//		Key = "boraText",
	//		Text = "Bora!"
	//	}
	//};

	//foreach (var homeContent in homeContents)
	//{
	//	homeContent.CreatedAt = DateTime.Now;
	//	homeContent.AccountId = 1;//lucasfogliarini
	//}

	//app.Services.Seed(homeContents);
}

static string? TryGetConnectionString(WebApplicationBuilder builder)
{
	var boraRepositoryConnectionStringKey = "ConnectionStrings:BoraRepository";
	Console.WriteLine($"Trying to get a database connectionString '{boraRepositoryConnectionStringKey}' from Configuration.");
	var connectionString = builder.Configuration[boraRepositoryConnectionStringKey];
	if (connectionString == null)
		throw new Exception($"{boraRepositoryConnectionStringKey} was not found! From builder.Configuration[{boraRepositoryConnectionStringKey}]");

	return connectionString;
}
