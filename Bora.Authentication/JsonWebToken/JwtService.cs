using Bora.Accounts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bora.Authentication.JsonWebToken
{
    public class JwtService(JwtConfiguration jwtConfiguration) : IJwtService
    {
        private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration;

        public Jwt CreateJwt(AuthenticationInput authenticationInput)
        {
            var tokenDescriptor = CreateTokenDescriptor(authenticationInput.Email, authenticationInput.Name);

            var jwt = new Jwt
            {
                Email = authenticationInput.Email,
                JwToken = GenerateToken(tokenDescriptor),
                ExpiresAt = tokenDescriptor.Expires.GetValueOrDefault()
            };

            return jwt;
        }
        private SecurityTokenDescriptor CreateTokenDescriptor(string email, string name)
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
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(_jwtConfiguration.SecurityKey), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }
        private static string GenerateToken(SecurityTokenDescriptor securityTokenDescriptor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            var key = Encoding.ASCII.GetBytes(securityKey!);
            return new SymmetricSecurityKey(key);
        }
    }
}
