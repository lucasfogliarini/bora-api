using Bora.Accounts;
using Bora.Database;
using Bora.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        private readonly IBoraDatabase _boraDatabase;
        private readonly IAccountService _accountService;
        private readonly Jwt _jwt;

        public TokenController(IBoraDatabase boraDatabase, IAccountService accountService, IOptions<Jwt> jwt)
        {
            _boraDatabase = boraDatabase;
            _accountService = accountService;
            _jwt = jwt.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Token(AuthenticationInput authenticationInput)
        {
            await _accountService.CreateOrUpdateAsync(authenticationInput);
            var authentication = await CreateAuthenticationAsync(authenticationInput);
            return Ok(authentication);
        }

        private async Task<Authentication> CreateAuthenticationAsync(AuthenticationInput authenticationInput)
        {
            var tokenDescriptor = _jwt.CreateTokenDescriptor(authenticationInput.Email, authenticationInput.Name);

            var authentication = new Authentication
            {
                Email = authenticationInput.Email,
                JwToken = _jwt.GenerateToken(tokenDescriptor),
                CreatedAt = DateTime.Now,
                ExpiresAt = tokenDescriptor.Expires.GetValueOrDefault(),
                Provider = authenticationInput.Provider
            };
            _boraDatabase.Add(authentication);
            await _boraDatabase.CommitAsync();
            return authentication;
        }
    }
}
