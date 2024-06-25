using Bora.Accounts;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public class AccountServiceTests : TestsBase
    {
        [Theory]
        [InlineData(ARQUITETO_EMAIL, false, false, "1990-07-17")]//1990-07-17 é o estado atual
        [InlineData(ARQUITETO_EMAIL, null, false, "1990-07-17")]//1990-07-17 é o estado atual
        [InlineData(ARQUITETO_EMAIL, true, true, "1990-07-17")]//1990-07-17 é o estado atual
        public async void UpdateAsync(string email, bool? isPartner, bool expectedIsPartner, string expectedPartnerSinceString)
        {
            //given
            var expectedPartnerSince = DateTime.Parse(expectedPartnerSinceString);
            var accountInput = new AccountInput
            {
                IsPartner = isPartner,
            };

            //when
            var accountService = _serviceProvider.GetService<IAccountService>()!;
            await accountService.UpdateAsync(email, accountInput);

            //then
            var accountAfter = accountService.GetAccount(email);
            Assert.Equal(expectedIsPartner, accountAfter.IsPartner);
            Assert.Equal(expectedPartnerSince.Date, accountAfter.PartnerSince.GetValueOrDefault().Date);
        }
    }
}