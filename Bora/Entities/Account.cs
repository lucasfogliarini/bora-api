using Azure;
using Azure.Data.Tables;
using System.Net.Mail;

namespace Bora.Entities
{
    public class Account(string email) : IEntity, ITableEntity
    {
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string PartitionKey { get; set; }

		public int Id { get; set; }
		public string Username { get; set; } = new MailAddress(email).User;
		public string? Accountability { get; set; }
        public string Name { get; set; }
		public string Email { get; set; } = email;
		public string? Photo { get; set; }
        public string? WhatsApp { get; set; }
        public string? Instagram { get; set; }
        public string? Linkedin { get; set; }
        public string? Spotify { get; set; }
        public string? YouTube { get; set; }
        public bool CalendarAuthorized { get; set; }
        public bool IsPartner { get; set; }
        public bool PartnerCommentsEnabled { get; set; }
        public bool PartnerCallsOpen { get; set; }
        public EventVisibility EventVisibility { get; set; }
        public bool OnlySelfOrganizer { get; set; }
        public string? CalendarAccessToken { get; set; }
        public string? CalendarRefreshAccessToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }		
	}
}
