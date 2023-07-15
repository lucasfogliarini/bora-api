namespace Bora.Events
{
    public class EventsCountOutput
    {
        public IEnumerable<ClosestAttendee> ClosestAttendees { get; set; }
        public IEnumerable<FavoriteLocation> FavoriteLocations { get; set; }
    }

    public class ClosestAttendee
    {
        public string Email { get; set; }
        public int AttendeeCount { get; set; }
        public int EventsCount { get; set; }
        public decimal ProximityRate
        {
            get
            {
                return AttendeeCount * 100 / EventsCount;
            }
        }
    }

    public class FavoriteLocation
    {
        public string Location { get; set; }
        public int Count { get; set; }
    }
}
