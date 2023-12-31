using Azure;
using Azure.Data.Tables;

namespace Bora.Entities
{
    public class Authentication : IEntity, ITableEntity
	{
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string PartitionKey { get; set; }

		public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public string JwToken { get; set; }
        public string Provider { get; set; }
    }
}
