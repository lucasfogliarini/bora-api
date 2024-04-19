using Bora.Accounts;
using Bora.Entities;
using BoraApi.Jwt;
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
            var authentication = _jwt.CreateAuthenticationToken(authenticationInput.Email, authenticationInput.Name);
            authentication.CreatedAt = DateTime.Now;
            authentication.Provider = authenticationInput.Provider;

			boraRepository.Add(authentication);
            await boraRepository.CommitAsync();
            return authentication;
        }
    }
}
