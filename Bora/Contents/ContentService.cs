using Bora.Accounts;
using Bora.Database;
using Bora.Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Contents
{
    public class ContentService : IContentService
    {
        private readonly IBoraDatabase _boraDatabase;
        private readonly IAccountService _accountService;

        public ContentService(IBoraDatabase boraDatabase, IAccountService accountService)
        {
            _boraDatabase = boraDatabase;
            _accountService = accountService;
        }
        public async Task UpdateAsync(string email, ContentInput contentInput)
        {
            _accountService.GetAccount(email);

            var content = _boraDatabase.Query<Content>()
                            .FirstOrDefault(e => e.Account.Email == email 
                            && e.Collection == contentInput.Collection 
                            && e.Key == contentInput.Key);

            if (content == null)
            {
                throw new ValidationException($"O conteúdo '{contentInput.Collection}' e '{contentInput.Key}' ainda não foi cadastrado para esse usuário.");
            }

            content.Text = contentInput.Text;
            content.UpdatedAt = DateTime.Now;
            _boraDatabase.Update(content);

            await _boraDatabase.CommitAsync();
        }
    }
}
