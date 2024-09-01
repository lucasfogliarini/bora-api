using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;

namespace Bora.Accounts
{
    public interface IAccountDataStore : IDataStore
    {
        Task<UserCredential> GetUserCredentialAsync(string username);
        Task AuthorizeCalendarAsync(string email, TokenResponse tokenResponse);
        Task UnauthorizeCalendarAsync(string email);
    }
}
