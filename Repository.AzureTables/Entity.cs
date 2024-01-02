using Azure;
using Azure.Data.Tables;

namespace Repository.AzureTables
{
	public class Entity : ITableEntity
	{
        public Entity()
        {
			InitCreatedAt(default);
		}
        public Entity(int? id, DateTimeOffset? createdAt = default)
		{
			Id = id;
			RowKey = id.ToString();
			InitCreatedAt(createdAt.GetValueOrDefault());
		}
        public string? RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string PartitionKey { get; set; }

		public int? Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get { return Timestamp; } }

		public int IncrementId(int? lastId)
		{
			Id = lastId ?? 0;
			Id++;
			RowKey = Id.ToString();
			return Id.Value;
		}

		private void InitCreatedAt(DateTimeOffset createdAt)
		{
			CreatedAt = createdAt == default ? DateTimeOffset.Now : createdAt;

			//if (ETag == default)
			//	ETag = new ETag(CreatedAt.ToString());
		}
	}
}
