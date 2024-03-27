using Bora.Events;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Bora.Accounts
{
	internal class AccountDataStore : IAccountDataStore
    {
        private readonly IRepository _boraRepository;
        private readonly IAccountService _accountService;
        private readonly GoogleCalendarConfiguration _googleCalendarConfiguration;

        public AccountDataStore(IRepository boraRepository,
                                IAccountService accountService,
                                GoogleCalendarConfiguration googleCalendarConfiguration)
        {
            _boraRepository = boraRepository;
            _accountService = accountService;
            _googleCalendarConfiguration = googleCalendarConfiguration;
        }

        public async Task<UserCredential> GetUserCredentialAsync(string username)
        {
            var account = _accountService.GetAccountByUsername(username);
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    _googleCalendarConfiguration.GoogleClientSecrets(),
                    [CalendarService.Scope.CalendarEvents],
                    account.Email,
                    CancellationToken.None,
                    this);

            return credential;
        }

        public async Task AuthorizeCalendarAsync(string email, TokenResponse tokenResponse)
        {
            var account = _accountService.GetAccount(email);
            account.CalendarAuthorized = true;
            account.CalendarAccessToken = tokenResponse.AccessToken;
            account.CalendarRefreshAccessToken = tokenResponse.RefreshToken;

            _boraRepository.Update(account);
            await _boraRepository.CommitAsync();
        }
        
        public async Task UnauthorizeCalendarAsync(string email)
        {
            var account = _accountService.GetAccount(email);
            account.CalendarAccessToken = null;
            account.CalendarRefreshAccessToken = null;
            account.CalendarAuthorized = false;

            _boraRepository.Update(account);
            await _boraRepository.CommitAsync();
        }

        public async Task<TTokenResponse> GetAsync<TTokenResponse>(string email)
        {
            var account = _accountService.GetAccount(email);
            if (account.CalendarAccessToken == null)
            {
                throw new ValidationException("Calendário não autorizado.");
            }

            var tokenResponse = new TokenResponse
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresInSeconds = TimeSpan.FromDays(300).Seconds,
                AccessToken = account.CalendarAccessToken,
                RefreshToken = account.CalendarRefreshAccessToken
            };

            string tokenResponseJsonString = JsonSerializer.Serialize(tokenResponse);
            TTokenResponse tokenResponseGeneric = JsonSerializer.Deserialize<TTokenResponse>(tokenResponseJsonString)!;
            return await Task.FromResult(tokenResponseGeneric);
        }

        public async Task StoreAsync<T>(string email, T tokenResponse)
        {
            if(tokenResponse is not TokenResponse)
            {
                throw new ValidationException("DataStore value is not a TokenResponse");
            }
            await RefreshCalendarAsync(email, tokenResponse as TokenResponse);
        }
        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }
        public Task DeleteAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        private async Task RefreshCalendarAsync(string email, TokenResponse tokenResponse)
        {
            var account = _accountService.GetAccount(email);
            if (account.UpdatedAt != null && account.UpdatedAt.Value.Date == DateTime.Today)
            {
                //avoid conccurrency
                return;
			}
            account.UpdatedAt = DateTime.Now;
			account.CalendarAuthorized = true;
			account.CalendarAccessToken = tokenResponse.AccessToken;
			account.CalendarRefreshAccessToken = tokenResponse.RefreshToken;

			_boraRepository.Update(account);
			await _boraRepository.CommitAsync();
		}
    }

    public interface IAccountDataStore : IDataStore
    {
        Task<UserCredential> GetUserCredentialAsync(string username);
        Task AuthorizeCalendarAsync(string email, TokenResponse tokenResponse);
        Task UnauthorizeCalendarAsync(string email);
    }
}
