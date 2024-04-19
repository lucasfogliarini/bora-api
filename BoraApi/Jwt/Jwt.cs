using Bora.Accounts;
using Bora.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoraApi.Jwt
{
    public class Jwt
    {
        public string? SecurityKey { get; set; }

        public Authentication CreateAuthenticationToken(string email, string name)
        {
            var tokenDescriptor = CreateTokenDescriptor(email, name);

            var authentication = new Authentication
            {
                Email = email,
                JwToken = GenerateToken(tokenDescriptor),
                ExpiresAt = tokenDescriptor.Expires.GetValueOrDefault()
            };

            return authentication;
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
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }
        public string GenerateToken(SecurityTokenDescriptor securityTokenDescriptor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            var key = Encoding.ASCII.GetBytes(SecurityKey!);
            return new SymmetricSecurityKey(key);
        }
    }
}
