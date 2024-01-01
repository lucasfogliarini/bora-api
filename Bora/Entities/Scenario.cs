namespace Bora.Entities
{
	public class Scenario : Entity
	{
        public string Title { get; set; }
        public string? Location { get; set; }

        public int? StartsInDays { get; set; }
        public bool? Public { get; set; }
        public string? Description { get; set; }
        public bool Enabled { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
