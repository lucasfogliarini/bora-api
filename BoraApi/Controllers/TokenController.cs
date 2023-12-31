using Bora.Accounts;
using Bora.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.AzureTables;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        private readonly IAzureTablesRepository _boraRepository;
        private readonly IAccountService _accountService;
        private readonly Jwt _jwt;

        public TokenController(IAzureTablesRepository boraRepository, IAccountService accountService, IOptions<Jwt> jwt)
        {
			_boraRepository = boraRepository;
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
			_boraRepository.Add(authentication);
            await _boraRepository.CommitAsync();
            return authentication;
        }
    }
}
