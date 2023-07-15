using Bora.Api;
using Bora.Database;
using Bora.Events;
using Bora.Database.Entities;
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
using Microsoft.EntityFrameworkCore;
using Spotify;

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

    var connectionString = TryGetConnectionString(builder);

	builder.Services.AddDatabase(connectionString);
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
    using var scope = app.Services.CreateScope();
    var boraDatabase = scope.ServiceProvider.GetService<BoraDbContext>();

    //update-database
    if(boraDatabase.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        boraDatabase.Database.Migrate();

    var accounts = new List<Account>
    {
        new Account("lucasfogliarini@gmail.com")
        {
            Name = "Lucas Fogliarini",
            Photo = "https://lh3.googleusercontent.com/a-/AOh14Ggingx4m5A-dFGLwEJv-acJ-KEDtApHCAO0NxfUig=s96-c",
            WhatsApp = "51992364249",
            Instagram = "lucasfogliarini",
            Spotify = "12145833562",
            CreatedAt = new DateTime(2022, 04, 01),
        },
        new Account("gandinirafaela@gmail.com")
        {
            Username = "rafagand",
            Name = "Rafaela Gandini",
            Instagram = "rafagand",
            WhatsApp = "51981583771",
            Spotify = "gandinir",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgCwlip_BZjPXJjj7_gA6Wu6Yqep1e6jXsP4i0FFxg=s96-c",
            CreatedAt = new DateTime(2022, 05, 03),
        },
        new Account("luanaleticiabueno@gmail.com")
        {
            Name = "Luana Bueno",
            WhatsApp = "5193840006",
            Instagram = "luanabuenoflores",
            Spotify = "224juavirzfsjsxt5yva6fvly",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhWN-zhlu_93Me88oT9v8554pdaJQdNYKpUp-i__c0=s340-p-k-rw-no",
            CreatedAt = new DateTime(2022, 04, 01),
        },
        new Account("divagandonosul@gmail.com")
        {
            Name = "Divagando",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgeKm5UynBAHY6Pl0jvBVvfFYM2A75KwIPuuS0=s96-c",
            WhatsApp = "51992364249",
            Instagram = "divagando.art",
            Spotify = "12145833562",
            CreatedAt = new DateTime(2022, 04, 01),
        },
        new Account("felipealmeida1395@gmail.com")
        {
            Name = "Felipe Almeida",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhohArS_wxlPMwCeHT8PmRO6Ts1oQSR4OvB8XrZFQ=s96-c",
            CreatedAt = new DateTime(2022, 04, 28),
        },
        new Account("psicologa.tanaia@gmail.com")
        {
            Name = "Tanaia Rodrigues",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Gi2StRGx59Iy5W1DXXoJsyzo0PsZF-_ZqI416NN=s96-c",
            CreatedAt = new DateTime(2022, 04, 29),
        },
        new Account("ricardoschieck@gmail.com")
        {
            Name = "Ricardo Schieck",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GiT4eOvOQI-vvBxGhxWLlRtteBMtXyICzAC1q45pQ=s96-c",
            CreatedAt = new DateTime(2022, 04, 30),
        },
        new Account("gui_staub@hotmail.com")
        {
            Name = "Guilherme Staub",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Gi14cQFSeyn5q6u3ZB_derhI7yIcA9dgX27OkBl=s96-c",
            CreatedAt = new DateTime(2022, 04, 30),
        },
        new Account("jonas.santos.correa1@gmail.com")
        {
            Name = "Jonas Correa",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJwOC7erXvrJHA70UJGsSd1JV1JB9T9BdcYbAFJH=s96-c",
            CreatedAt = new DateTime(2022, 05, 02),
            CalendarAuthorized= true,
            CalendarAccessToken = "ya29.a0ARrdaM8qKQzZYL3weEDeVrgVEjXzEyhjNpmDoOW1DFXKrT3H4mKWrHHaJd1scz5u_vgmySPnOFgISHjW0HNSOXUOr8FvFHH-FSiOi-ev40oSAvQqc_l6FMDFs1TcUuC9yoTIWPqjkxXD3HtxUoTjWa8fu-t0",
            CalendarRefreshAccessToken = "1//0hnnvcOlqZfz7CgYIARAAGBESNwF-L9Ir7a1TLnnkl_p_EUk413j6McA_If2XbIBqEtwoWVnBNqt2WggGa39QrLlXFG6yUYVP7d0"
        },
        new Account("eds.pedroso.58@gmail.com")
        {
            Name = "Edson Pedroso",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJxnL21olUcIhwApbEFncNt7f37EshRf3f_we5PB=s96-c",
            CreatedAt = new DateTime(2022, 05, 02),
        },
        new Account("varreira.adv@gmail.com")
        {
            Name = "Anderson Varreira",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJw-6J_C5vAh-d9Gp3ssN_ziJrOkzp6HMWXE6Ubm=s96-c",
            CreatedAt = new DateTime(2022, 05, 03),
        },
        new Account("rafaellalacerda17m@gmail.com")
        {
            Name = "Rafaella Lacerda",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhqCaFO-uDGlrAN2h9--Kkeu5WxpRiTqDMZomlgrw=s96-c",
            CreatedAt = new DateTime(2022, 05, 03),
        },
        new Account("danielsantanaschaefer@gmail.com")
        {
            Name = "Daniel Schaefer",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GjREHDPMEu48vlof4ZiyDOL1_R6JqffJnLm7euzbjY=s96-c",
            CreatedAt = new DateTime(2022, 05, 03),
        },
        new Account("lopes.laura95@gmail.com")
        {
            Name = "Laura Lopes",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgspOyvcp_TAKmEzNh23I3qnlXLJwpKegx5iGOMnw=s96-c",
            CreatedAt = new DateTime(2022, 05, 07),
        },
        new Account("lucasbuenomagalhaes@gmail.com")
        {
            Name = "Lucas Bueno",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggfyxso7uuqWxLMqvI3JTDOcKDRKkOgsz0oOwLWPw=s96-c",
            CreatedAt = new DateTime(2022, 05, 08),
        },
        new Account("b.vargash21@gmail.com")
        {
            Name = "Bruno Vargas",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgRTDTt5rVSKic_1k2hGcyMmwDPQfsitYtyvkMu=s96-c",
            CreatedAt = new DateTime(2022, 05, 12),
        },
        new Account("vitoriarqoue2000@gmail.com")
        {
            Name = "Vitória Roque",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhJt54oTmQuzjtQJqmFk6FDEvcqFRUepgZowZiHadI=s96-c",
            CreatedAt = new DateTime(2022, 06, 11),
        },
        new Account("pavezivinicius@gmail.com")
        {
            Name = "Vinícius Pavezi",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhOhDPKfy6jb97uWflm1q367lOzkHpwWQn7JCufKA=s96-c",
            CreatedAt = new DateTime(2022, 06, 14),
        },
        new Account("debora.blara19@gmail.com")
        {
            Name = "Débora Borges",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GiMfj386GjT5kY-M45LVAb2dxbjtsNZJnI3kMrXdg=s96-c",
            CreatedAt = new DateTime(2022, 06, 14),
        },
        new Account("singutar2000@gmail.com")
        {
            Name = "Samuel de mello",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgTJT1DB6Qcz8kGeI6-8jk8amKuGfkhg37i6SJKB3w=s96-c",
            CreatedAt = new DateTime(2022, 06, 16),
        },
        new Account("gabee.roggia@gmail.com")
        {
            Name = "gabriel roggia",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJwGtHK4oFqOAAA-vKWuXdbhvhpv5uIM_WyQerEGUg=s96-c",
            CreatedAt = new DateTime(2022, 06, 18),
        },
        new Account("pfd0709@gmail.com")
        {
            Name = "Pietro Dias",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhZf_xVV0IB8CiRA1lmrV2CVFBSNt4Q1cB2uPUL=s96-c",
            CreatedAt = new DateTime(2022, 06, 18),
        },
        new Account("lucasteyding@gmail.com")
        {
            Name = "Lucas Steyding",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJxI6sYBxzZESj6rWVleCalsxZelO1Z6H7t6dokZ=s96-c",
            CreatedAt = new DateTime(2022, 06, 18),
        },
        new Account("matheusfrankehoppe@gmail.com")
        {
            Name = "Matheus Franke Hoppe",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgtoskZZ3jWjdMl4GNuEBwRo_wROB1cZ4ubmVHM=s96-c",
            CreatedAt = new DateTime(2022, 06, 19),
        },
        new Account("anderson.zaniratti@gmail.com")
        {
            Name = "Anderson Zaniratti",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggj_yvm15rD-aWzhFvFX4AyAIigUdJjPQRTbS2icg=s96-c",
            CreatedAt = new DateTime(2022, 06, 19),
        },
        new Account("vetteragencia@gmail.com")
        {
            Name = "Vetter Ag\u00eancia",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggtd6dZGV4_Bgnaub0k0RPHEWljEjGi-7k1kYTc=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("restaurantepavilhao@gmail.com")
        {
            Name = "Pavilh\u00e3o Porto Alegre",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Gikq53Jv4Bbdhmm2CMlZuEOjwi6KlD3h7dkBgs=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("diegoluismacedo@gmail.com")
        {
            Name = "DSK - Diego Soker",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJxRtUMxosvJIgXtG2YPjz-y8t5OC24ORele9zR2ySw=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("ezequiel.dartora92@gmail.com")
        {
            Name = "Ezequiel Dartora",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJxIi18cfJjurl5hhk6LV910RwGpcmqww-X8HnTA=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("fabiolandskron@gmail.com")
        {
            Name = "Fabio Landskron",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Gh-1uugkO-7FUOTqlSh7kyak7pYp8Adz4IeDsFY=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("pedrokroth@mx2.unisc.br")
        {
            Name = "Pedro Kroth",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GhvRt-KRX6wD-KHlYuDU22_On_Weu7MsvfkqjsE=s96-c",
            CreatedAt = new DateTime(2022, 06, 22),
        },
        new Account("pmarinho85@gmail.com")
        {
            Name = "Paulo Jos\u00e9 Marinho Dias",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgLhk2eEUMK51gRMI-Zm2VFG0_PF7IPGJLZh0snew=s96-c",
            CreatedAt = new DateTime(2022, 06, 23),
        },
        new Account("karolzinha_hmt_ibias@hotmail.com")
        {
            Name = "Ana Caroline Ibias",
            Photo= "https://lh3.googleusercontent.com/a/AATXAJw_WQIgZKm-z2FjeP522u5E-c4Sg3tff_2zqmt3=s96-c",
            CreatedAt = new DateTime(2022, 06, 23),
        },
        new Account("rodrigoschieck.pro@gmail.com")
        {
            Name = "Rodrigo Schieck",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Gjikfm33H1HZqqwzSV10X1H1ZQGUuA5hqo15fY0Zw=s96-c",
            CreatedAt = new DateTime(2022, 06, 24),
        },
        new Account("grazielepintoribeiro@gmail.com")
        {
            Name = "Graziele Ribeiro",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14Ghs7ympBmcup2kObibBUkfohX-uFNi0YAHuQiZrOQ=s96-c",
            CreatedAt = new DateTime(2022, 06, 24),
        },
        new Account("eliveltondrey296@gmail.com")
        {
            Name = "Crypto Tech",
            Photo= "https://lh3.googleusercontent.com/a-/AOh14GgELyJQp7QJ2LaET5NmbRLefTT_O4rxEc0HjDNAaw=s96-c",
            CreatedAt = new DateTime(2022, 06, 24),
        }
    };
    //boraDatabase.AddRange(accounts);

    var homeContents = new List<Content>
    {
        new Content
        {
            Collection = "home",
            Key = "title",
            Text = "Divagando"
        },
        new Content
        {
            Collection = "home",
            Key = "subtitle",
            Text = "no Camarote"
        },
        new Content
        {
            Collection = "home",
            Key = "bora",
            Text = "Bora!"
        },
        new Content
        {
            Collection = "home",
            Key = "presentation",
            Text = "Celebre momentos com amigos e familiares"
        },
        new Content
        {
            Collection = "home",
            Key = "presentationDescription",
            Text = "Compartilhe seus melhores momentos com amigos e familiares enviando convites para: viagens, festas, risadas, almoços e jantares especiais."
        },
        new Content
        {
            Collection = "home",
            Key = "featureGoogleAgendaTitle",
            Text = "Integração com Google Agenda"
        },
        new Content
        {
            Collection = "home",
            Key = "featureGoogleAgendaDescription",
            Text = "Crie um perfil e publique seus eventos do Google Agenda"
        },
        new Content
        {
            Collection = "home",
            Key = "featureBoraTitle",
            Text = "Visualize quem são os convidados dos eventos e participe!"
        },
        new Content
        {
            Collection = "home",
            Key = "featureDomainTitle",
            Text = "Domínio customizado"
        },
        new Content
        {
            Collection = "home",
            Key = "featureDomainDescription",
            Text = "Adicione seu domínio customizado (minhapagina.com) e crie um perfil com foto, links de redes sociais e eventos do Google Agenda."
        },
        new Content
        {
            Collection = "home",
            Key = "featureLocationTitle",
            Text = "Localização"
        },
        new Content
        {
            Collection = "home",
            Key = "featureLocationDescription",
            Text = "Veja a localização dos eventos e navegue pelo Google Maps"
        },
        new Content
        {
            Collection = "home",
            Key = "featureTicketTitle",
            Text = "Ingresso"
        },
        new Content
        {
            Collection = "home",
            Key = "featureTicketDescription",
            Text = "Acesse o ingresso do evento"
        },
        new Content
        {
            Collection = "home",
            Key = "featureShareTitle",
            Text = "Compartilhe"
        },
        new Content
        {
            Collection = "home",
            Key = "featureShareDescription",
            Text = "Compartilhe eventos por WhatsApp"
        },
        new Content
        {
            Collection = "home",
            Key = "featureMediaTitle",
            Text = "Spotify"
        },
        new Content
        {
            Collection = "home",
            Key = "featureMediaDescription",
            Text = "Compartilhe uma música ou playlist do Spotify no seu evento"
        }
    };

    foreach (var homeContent in homeContents)
    {
        homeContent.CreatedAt = DateTime.Now;
        homeContent.AccountId = 1;//lucasfogliarini!?
    }

    //boraDatabase.AddRange(homeContents);

    //await boraDatabase.SaveChangesAsync();
}

static string? TryGetConnectionString(WebApplicationBuilder builder)
{
	var connectionStringName = "boraDatabase";
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
