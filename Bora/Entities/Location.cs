namespace Bora.Entities
{
	public class Location : Entity
	{
        public Location(string name, string? place = null)
        {
            Name = name;
            Place = place ?? name;
            CreatedAt = DateTime.Now;
            Enabled = true;
        }
        public string Name { get; set; }
        public string? Place { get; set; }
        public bool Enabled { get; set; }
        public bool IsHome { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
