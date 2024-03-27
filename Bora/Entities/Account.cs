using System.Net.Mail;

namespace Bora.Entities
{
	public class Account : Entity
    {
        public Account()
        {
        }
        public Account(string email, int? id = null, DateTimeOffset? createdAt = null) : base(id, createdAt)
		{
            Email = email;
            Username = new MailAddress(email).User;
		}
        public string Username { get; set; }
		public string? Accountability { get; set; }
        public string Name { get; set; }
		public string Email { get; set; }
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
	}
}
