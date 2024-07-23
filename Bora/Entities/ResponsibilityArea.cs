namespace Bora.Entities
{
	public class ResponsibilityArea(string title, string description) : Entity
	{
        public string Title { get; set; } = title;
        public string Description { get; set; } = description;
    }
}
