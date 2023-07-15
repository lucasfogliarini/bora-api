namespace Bora.Database.Entities
{
    public class Authentication : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public string JwToken { get; set; }
        public string Provider { get; set; }
    }
}
