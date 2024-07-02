using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        public DateTimeOffset? PartnerSince { get; set; }
        public bool IsPartner { get; set; }
        public bool PartnerCommentsEnabled { get; set; }
        public bool PartnerCallsOpen { get; set; }
        public bool CalendarAuthorized { get; set; }
        [IgnoreDataMember]
        public string? CalendarAccessToken { get; set; }
        [IgnoreDataMember]
        public string? CalendarRefreshAccessToken { get; set; }
        public string Username { get; set; }
		public string? Accountability { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
		public string? Photo { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string? WhatsApp { get; set; }
        public string? Linkedin { get; set; }
        public string? Github { get; set; }
        public string? HorizonMeta { get; set; }
        public string? Tribe { get; set; }
        public string? Chess { get; set; }
        public string? Instagram { get; set; }
        public string? Spotify { get; set; }
        public string? YouTube { get; set; }
        public EventVisibility EventVisibility { get; set; }
        /// <summary>
        /// Returns only events organized by the account owner.
        /// </summary>
        public bool OnlySelfOrganizer { get; set; }
        
	}
}
