namespace Bora.JsonWebToken
{
    public class Jwt
    {
        public required string Email { get; set; }
        public required string JwToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
