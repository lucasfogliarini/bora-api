namespace Bora.Events
{
    public class EventInput
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Location { get; set; }
        public bool? Public { get; set; }
        public bool AddConference { get; set; } = true;
    }
}
