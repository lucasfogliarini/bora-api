using Bora.Entities;

namespace Bora.Accounts
{
    public interface IAccountService
    {

        /// <summary>
        /// Get Account by e-mail
        /// </summary>
        /// <param name="email">Account Email</param>
        /// <exception cref="ValidationException">Usuário não existe</exception>
        Account GetAccount(string email);
        /// <summary>
        /// Get Account by username
        /// </summary>
        /// <param name="username">Account username</param>
        /// <exception cref="ValidationException">Usuário não existe</exception>
        Account GetAccountByUsername(string username);
        Task UpdateAsync(string email, AccountInput account);
        Task CreateOrUpdateAsync(AuthenticationInput authenticationInput);
    }
}
