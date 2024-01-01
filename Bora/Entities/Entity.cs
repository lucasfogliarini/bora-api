using Azure;
using Azure.Data.Tables;

namespace Bora.Entities
{
    public interface IEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

	public abstract class Entity : IEntity, ITableEntity
	{
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string PartitionKey { get; set; }

		public int Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get { return Timestamp; } }
	}
}
