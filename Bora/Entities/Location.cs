using Azure;
using Azure.Data.Tables;

namespace Bora.Entities
{
    public class Location : IEntity, ITableEntity
	{
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string PartitionKey { get; set; }

		public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
