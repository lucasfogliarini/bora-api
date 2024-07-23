namespace Bora.Entities
{
	public class Responsibility(string title, int areaId, string description) : Entity
	{
        public string Title { get; set; } = title;
        public int AreaId { get; set; } = areaId;
        public ResponsibilityArea Area { get; set; }
        public string Description { get; set; } = description;
    }
}
