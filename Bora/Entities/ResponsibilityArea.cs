namespace Bora.Entities
{
	public class ResponsibilityArea(string title, string description) : Entity
	{
        public const int EarthScience = 1;
        public const int HumanScience = 2;
        public const int TecnologyScience = 3;
        public const int AppliedSocial = 4;
        public const int HealthScience = 5;
        public const int BiologicalScience = 6;
        public string Title { get; set; } = title;
        public string Description { get; set; } = description;
    }
}
