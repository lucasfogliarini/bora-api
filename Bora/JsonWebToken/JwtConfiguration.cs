namespace Bora.JsonWebToken
{
    public class JwtConfiguration
    {
        public const string JwtSection = "Jwt";
        public required string SecurityKey { get; set; }
    }
}
