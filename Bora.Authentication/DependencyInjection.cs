using Bora.Authentication.JsonWebToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddJwtAuthentication(this IServiceCollection services, JwtConfiguration jwtConfiguration)
        {
            services.AddJwtService(jwtConfiguration);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtService.GetSymmetricSecurityKey(jwtConfiguration.SecurityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
        private static IServiceCollection AddJwtService(this IServiceCollection serviceCollection, JwtConfiguration jwtConfiguration)
        {
            serviceCollection.AddScoped<IJwtService, JwtService>();
            serviceCollection.AddSingleton(jwtConfiguration);
            return serviceCollection;
        }
    }
}
