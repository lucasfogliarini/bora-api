using Bora.Accounts;
using Bora.Authentication.JsonWebToken;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController(IRepository boraRepository, IAccountService accountService, IJwtService jwtService) : BaseController
    {
		[HttpPost]
        public async Task<IActionResult> Token(AuthenticationInput authenticationInput)
        {
            await accountService.CreateOrUpdateAsync(authenticationInput);
            var jwt = await AuthenticateAsync(authenticationInput);
            
            return Ok(jwt);
        }

        private async Task<Jwt> AuthenticateAsync(AuthenticationInput authenticationInput)
        {
            var jwt = jwtService.CreateJwt(authenticationInput);
            await accountService.AuthenticateAsync(jwt, authenticationInput.Provider);
            return jwt;
        }
    }
}
