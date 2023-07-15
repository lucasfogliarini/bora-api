namespace Bora.Events
{
    public interface IEventService
    {
        Task<IEnumerable<EventOutput>> EventsAsync(string user, EventsFilterInput eventsFilter);
        Task<EventsCountOutput> EventsCountAsync(string user);
        Task<EventOutput> CreateAsync(string user, EventInput eventInput, AttendeeInput attendeeInput);
        Task<EventOutput> UpdateAsync(string user, string eventId, EventInput eventInput);
        Task<EventOutput> ReplyAsync(string user, string eventId, AttendeeInput attendeeInput);
    }
}
