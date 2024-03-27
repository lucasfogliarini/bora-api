namespace Bora.Entities
{
	public class Content : Entity
	{
        public string Collection { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
