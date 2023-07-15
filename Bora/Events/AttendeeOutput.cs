namespace Bora.Events
{
    public class AttendeeOutput
    {
        internal string Email { get; init; }
        public string? Username { get; init; }
        public string? Comment { get; set; }
        public string? Name { get; init; }
        public string? Photo { get; init; }
        public string? Instagram { get; init; }
        public string? WhatsApp { get; init; }
        public string? Spotify { get; init; }
        public bool IsPartner { get; set; }
        public decimal? ProximityRate { get; internal set; }
        
    }
}
