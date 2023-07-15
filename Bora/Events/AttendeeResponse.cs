namespace Bora.Events
{
    public enum AttendeeResponse
    {
        Declined,
        Accepted,
        Tentative
    }

    public static class AttendeeResponseExtensions
    {
        public static string ToResponseStatus(this AttendeeResponse? attendeeResponse)
        {
            return attendeeResponse?.ToString().ToLower();
        }
    }
}
