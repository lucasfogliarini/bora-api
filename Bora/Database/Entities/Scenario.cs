namespace Bora.Database.Entities
{
    public class Scenario : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }

        public int? StartsInDays { get; set; }
        public bool? Public { get; set; }
        public string? Description { get; set; }
        public bool Enabled { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
