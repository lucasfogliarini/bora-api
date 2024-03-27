using Bora.Accounts;
using Bora.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Contents
{
	public class ContentService : IContentService
    {
		private readonly IRepository _boraRepository;
		private readonly IAccountService _accountService;

        public ContentService(IRepository boraRepository, IAccountService accountService)
        {
            _boraRepository = boraRepository;
            _accountService = accountService;
        }
        public async Task UpdateAsync(string email, ContentInput contentInput)
        {
            _accountService.GetAccount(email);

            var content = _boraRepository.FirstOrDefault<Content>(e => e.Account.Email == email 
                            && e.Collection == contentInput.Collection 
                            && e.Key == contentInput.Key);

            if (content == null)
            {
                throw new ValidationException($"O conteúdo '{contentInput.Collection}' e '{contentInput.Key}' ainda não foi cadastrado para esse usuário.");
            }

            content.Text = contentInput.Text;
            _boraRepository.Update(content);

            await _boraRepository.CommitAsync();
        }
    }
}
