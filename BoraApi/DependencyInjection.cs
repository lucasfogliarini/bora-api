using Bora.Accounts;
using Bora.Authentication.JsonWebToken;
using Bora.Events;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;

namespace BoraApi
{
    public static class DependencyInjection
    {
        public static void AddServices(this WebApplicationBuilder builder)
        {
            var jwtSection = builder.Configuration.GetSection(JwtConfiguration.JwtSection);
            var jwtConfiguration = jwtSection.Get<JwtConfiguration>()!;
            builder.Services.AddJwtAuthentication(jwtConfiguration);

            builder.Services.AddBoraServices();

            builder.AddGoogleCalendar();
        }
        public static void AddGoogleCalendar(this WebApplicationBuilder builder)
        {
            var googleCalendarSection = builder.Configuration.GetSection(GoogleCalendarConfiguration.GoogleCalendarSection);
            var googleCalendarConfig = googleCalendarSection.Get<GoogleCalendarConfiguration>()!;
            builder.Services.AddGoogleCalendarService(googleCalendarConfig);

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
        public static void AddSwaggerJwtAuthentication(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        }
    }
}
