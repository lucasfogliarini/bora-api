namespace Bora.Entities
{
	public class Location : Entity
	{
        public string Name { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
