using Bora.Accounts;
using Bora.Entities;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class AccountsController : ODataController<Account>
    {
        private readonly IAccountService _accountService;
        private readonly IAccountDataStore _accountDataStore;

        public AccountsController(IRepository boraRepository,
                                  IAccountService accountService,
                                  IAccountDataStore accountDataStore) : base(boraRepository)
        {
            _accountDataStore = accountDataStore;
            _accountService = accountService;
        }

        /// <summary>
        /// Inicia o processo de autorização usando OAuth, permitindo que sua aplicação obtenha as permissões necessárias para acessar o serviço do Google Calendar com o seu usuário.
        ///  
        /// Permissões:
        /// See, edit, share, and permanently delete all the calendars you can access using Google Calendar
        /// scope: https://www.googleapis.com/auth/calendar
        /// </summary>
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

        /// <summary>
        /// Inicia o processo de autorização usando OAuth, permitindo que sua aplicação obtenha as permissões necessárias para acessar o serviço do Google Calendar.
        ///  
        /// Permissões:
        /// View events on all your calendars
        /// scope: https://www.googleapis.com/auth/calendar.events.readonly
        /// </summary>
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
