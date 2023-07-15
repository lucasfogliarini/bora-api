namespace Bora.Database.Entities
{
    public class Content : IEntity
    {
        public int Id { get; set; }
        public string Collection { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
