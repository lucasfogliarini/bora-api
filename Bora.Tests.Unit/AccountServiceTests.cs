using Bora.Accounts;
using Bora.Authentication.JsonWebToken;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public class AccountServiceTests : TestsBase
    {
        [Theory]
        [InlineData(ARQUITETO_EMAIL, "Lucas Fogliarini", "img1", "GOOGLE")]
        public async Task AuthenticateAsync(string email, string name, string photoUrl, string provider)
        {
            //given
            var authenticationInput = new AuthenticationInput
            {
                Email = email,
                Name = name,
                PhotoUrl = photoUrl,
                Provider = provider
            };

            //when
            var jwtService = _serviceProvider.GetService<IJwtService>()!;
            var accountService = _serviceProvider.GetService<IAccountService>()!;

            var jwt = jwtService.CreateJwt(authenticationInput);
            var authentication = await accountService.AuthenticateAsync(jwt, authenticationInput.Provider);

            //then
            Assert.Equal(authentication.Email, email);
            Assert.Equal(authentication.Provider, provider);
            Assert.Equal(authentication.JwToken, jwt.JwToken);
            Assert.Equal(authentication.ExpiresAt, jwt.ExpiresAt);
            Assert.Equal(authentication.CreatedAt.Date, DateTime.Today);
        }

        [Theory]
        [InlineData(ARQUITETO_EMAIL, false, null, false, "1990-07-17")]//1990-07-17 é o estado atual
        [InlineData(ARQUITETO_EMAIL, null, null, false, "1990-07-17")]//1990-07-17 é o estado atual
        [InlineData(ARQUITETO_EMAIL, true, 4, true, "1990-07-17")]//1990-07-17 é o estado atual
        [InlineData(ARQUITETO_EMAIL, true, null, true, "1990-07-17")]//1990-07-17 é o estado atual
        public async void UpdateAsync(string email, bool? isPartner, int? responsibilityArea, bool expectedIsPartner, string expectedPartnerSinceString)
        {
            //given
            var expectedPartnerSince = DateTime.Parse(expectedPartnerSinceString);
            var accountInput = new AccountInput
            {
                IsPartner = isPartner,
                ResponsibilityArea = responsibilityArea
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