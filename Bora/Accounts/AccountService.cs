using Bora.Database;
using Bora.Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IBoraDatabase _boraDatabase;

        public AccountService(IBoraDatabase boraDatabase)
        {
            _boraDatabase = boraDatabase;
        }

        public Account GetAccount(string email)
        {
            var account = _boraDatabase.Query<Account>().FirstOrDefault(e => e.Email == email);
            if (account == null)
            {
                throw new ValidationException("Usuário não existe.");
            }
            return account;
        }
        public Account GetAccountByUsername(string username)
        {
            var account = _boraDatabase.Query<Account>().FirstOrDefault(e => e.Username == username);
            if (account == null)
            {
                throw new ValidationException("Usuário não existe.");
            }
            return account;
        }
        public async Task CreateOrUpdateAsync(AuthenticationInput authenticationInput)
        {
            var account = _boraDatabase.Query<Account>().FirstOrDefault(e => e.Email == authenticationInput.Email);
            if (account == null)
            {
                account = new Account(authenticationInput.Email)
                {
                    Name = authenticationInput.Name,
                    CreatedAt = DateTime.Now,
                    Photo = authenticationInput.PhotoUrl
                };
                _boraDatabase.Add(account);
            }
            else
            {
                //account.Name = authenticationInput.Name;
                //account.Photo = authenticationInput.PhotoUrl;
                account.UpdatedAt = DateTime.Now;//last login at
                _boraDatabase.Update(account);
            }
            await _boraDatabase.CommitAsync();
        }
        public async Task UpdateAsync(string email, AccountInput accountInput)
        {
            var account = GetAccount(email);
            if (account != null)
            {
                account.UpdatedAt = DateTime.Now;
                if (accountInput.Accountability != null)
                    account.Accountability = accountInput.Accountability;
                if (accountInput.Name != null)
                    account.Name = accountInput.Name;
                if (accountInput.Photo != null)
                    account.Photo = accountInput.Photo;
                if (accountInput.WhatsApp != null)
                    account.WhatsApp = accountInput.WhatsApp;
                if (accountInput.Instagram != null)
                    account.Instagram = accountInput.Instagram;
                if (accountInput.Spotify != null)
                    account.Spotify = accountInput.Spotify;
                if (accountInput.Linkedin != null)
                    account.Linkedin = accountInput.Linkedin;
                if (accountInput.YouTube != null)
                    account.YouTube = accountInput.YouTube;
                if (accountInput.IsPartner != null)
                    account.IsPartner = accountInput.IsPartner.Value;

                _boraDatabase.Update(account);
                await _boraDatabase.CommitAsync();

                await ValidateAndUpdateUsername(account, accountInput.Username);
            }
        }

        private async Task ValidateAndUpdateUsername(Account account, string? newUsername)
        {
            if (newUsername != null)
            {
                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    throw new ValidationException($"O usuário deve ter pelo menos 1 caractere.");
                }
                var userNameAlreadyTaken = _boraDatabase.Query<Account>().Any(e => e.Username == newUsername && e.Email != account.Email);
                if (userNameAlreadyTaken)
                {
                    throw new ValidationException($"O usuário '{newUsername}' já está sendo usado.");
                }
                account.Username = newUsername;
                _boraDatabase.Update(account);
                await _boraDatabase.CommitAsync();
            }
        }
    }
}
