namespace Bora.Events
{
    public class EventOutput
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Location { get; set; }
        public DateTime? Deadline { get; set; }
        public IEnumerable<AttendeeOutput>? Attendees { get; set; }
        public string? Chat { get; set; }
        public string? ConferenceUrl { get; set; }
        public string? TicketUrl { get; set; }
        public string? SpotifyUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string[]? Attachments { get; set; }
        public string? GoogleEventUrl { get; set; }
        public bool Public { get; set; }
    }
}
