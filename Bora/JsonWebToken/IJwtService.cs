using Bora.Accounts;

namespace Bora.JsonWebToken
{
    public interface IJwtService
    {
        public Jwt CreateJwt(AuthenticationInput authenticationInput);
    }
}
