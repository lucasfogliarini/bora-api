using Bora.Accounts;
using Bora.Entities;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Repository.AzureTables;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ODataController<Account>
    {
        private readonly IAccountService _accountService;
        private readonly IAccountDataStore _accountDataStore;

        public AccountsController(IAzureTablesRepository boraRepository,
                                  IAccountService accountService,
                                  IAccountDataStore accountDataStore) : base(boraRepository)
        {
            _accountDataStore = accountDataStore;
            _accountService = accountService;
        }

        [HttpGet("authorizeCalendar")]
        [GoogleScopedAuthorize(CalendarService.ScopeConstants.Calendar)]
        public IActionResult AuthorizeCalendarAsync(string? redirectUrl)
        {
            if(redirectUrl == null)
            {
                return NoContent();
            }
            return Redirect(redirectUrl);
        }

        [HttpGet("authorizeEvents")]
        [GoogleScopedAuthorize(CalendarService.ScopeConstants.CalendarEventsReadonly)]
        public IActionResult AuthorizeEventsAsync()
        {
            return Ok("autorizado.");
        }

        [HttpPatch("unauthorizeCalendar")]
        public async Task<IActionResult> UnauthorizeCalendarAsync()
        {
            await _accountDataStore.UnauthorizeCalendarAsync(AuthenticatedUserEmail);
            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAsync(AccountInput accountInput)
        {
            await _accountService.UpdateAsync(AuthenticatedUserEmail, accountInput);
            return Ok();
        }
    }
}
