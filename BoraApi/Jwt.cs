using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bora.Api
{
    public class Jwt
    {
        public const string AppSettingsKey = "Jwt";
        public string? SecurityKey { get; set; }

        public static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection(AppSettingsKey);
            services.Configure<Jwt>(jwtSection);

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
                    IssuerSigningKey = GetSymmetricSecurityKey(jwtSection.Get<Jwt>().SecurityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public SecurityTokenDescriptor CreateTokenDescriptor(string email, string name)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //more info in https://balta.io/artigos/aspnet-5-autenticacao-autorizacao-bearer-jwt#autorizando
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Email, email),
                    new(ClaimTypes.Name, name)
                }),
                Expires = DateTime.UtcNow.AddMonths(6),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(SecurityKey!), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }

        public string GenerateToken(SecurityTokenDescriptor securityTokenDescriptor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static SymmetricSecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            var key = Encoding.ASCII.GetBytes(securityKey);
            return new SymmetricSecurityKey(key);
        }
    }
}
