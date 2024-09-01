using Bora.Accounts;

namespace Bora.Authentication.JsonWebToken
{
    public interface IJwtService
    {
        public Jwt CreateJwt(AuthenticationInput authenticationInput);
    }
}
