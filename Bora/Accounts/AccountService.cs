using Bora.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Accounts
{
	public class AccountService : IAccountService
    {
		private readonly IRepository _boraRepository;

		public AccountService(IRepository boraRepository)
        {
            _boraRepository = boraRepository;
        }

        public Account GetAccount(string email)
        {
            var account = _boraRepository.Where<Account>(e => e.Email == email).FirstOrDefault();
            if (account == null)
            {
                throw new ValidationException("Usuário não existe.");
            }
            return account;
        }
        public Account GetAccountByUsername(string username)
        {
            var account = _boraRepository.FirstOrDefault<Account>(e => e.Username == username);
            if (account == null)
            {
                throw new ValidationException("Usuário não existe.");
            }
            return account;
        }
        public async Task CreateOrUpdateAsync(AuthenticationInput authenticationInput)
        {
            var account = _boraRepository.FirstOrDefault<Account>(e => e.Email == authenticationInput.Email);
            if (account == null)
            {
                account = new Account(authenticationInput.Email)
                {
                    Name = authenticationInput.Name,
                    CreatedAt = DateTime.Now,
                    Photo = authenticationInput.PhotoUrl
                };
                _boraRepository.Add(account);
            }
            else
            {
                //account.Name = authenticationInput.Name;
                //account.Photo = authenticationInput.PhotoUrl;
                _boraRepository.Update(account);
            }
            await _boraRepository.CommitAsync();
        }
        public async Task UpdateAsync(string email, AccountInput accountInput)
        {
            var account = GetAccount(email);
            if (account != null)
            {
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

                _boraRepository.Update(account);
                await _boraRepository.CommitAsync();

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
                var userNameAlreadyTaken = _boraRepository.Where<Account>(e => e.Username == newUsername && e.Email != account.Email).Any();
                if (userNameAlreadyTaken)
                {
                    throw new ValidationException($"O usuário '{newUsername}' já está sendo usado.");
                }
                account.Username = newUsername;
                _boraRepository.Update(account);
                await _boraRepository.CommitAsync();
            }
        }
    }
}
