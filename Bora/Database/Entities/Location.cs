namespace Bora.Database.Entities
{
    public class Location : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
