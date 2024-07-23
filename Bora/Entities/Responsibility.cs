namespace Bora.Entities
{
	public class Responsibility(string title) : Entity
	{
        public string Title { get; set; } = title;
        public int AreaId { get; set; }
        public ResponsibilityArea Area { get; set; }
    }
}
