namespace Bora.Entities
{
	public class Authentication : Entity
	{
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public string JwToken { get; set; }
        public string Provider { get; set; }
    }
}
