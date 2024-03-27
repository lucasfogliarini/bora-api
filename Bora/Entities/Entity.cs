namespace Bora.Entities
{
	public class Entity(int? id = null, DateTimeOffset? createdAt = null)
	{
		public int? Id { get; set; } = id;
		public DateTimeOffset CreatedAt { get; set; } = createdAt ?? DateTime.Now;
		public DateTimeOffset? UpdatedAt { get; set; }
	}
}
