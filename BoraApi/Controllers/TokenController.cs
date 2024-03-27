using Bora.Accounts;
using Bora.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class TokenController(IRepository boraRepository, IAccountService accountService, IOptions<Jwt> jwt) : BaseController
    {
		private readonly Jwt _jwt = jwt.Value;

		[HttpPost]
        public async Task<IActionResult> Token(AuthenticationInput authenticationInput)
        {
            await accountService.CreateOrUpdateAsync(authenticationInput);
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
			boraRepository.Add(authentication);
            await boraRepository.CommitAsync();
            return authentication;
        }
    }
}
