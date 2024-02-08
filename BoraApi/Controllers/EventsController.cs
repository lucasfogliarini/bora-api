using Bora.Events;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : BaseController
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

		[HttpGet]
        public async Task<IActionResult> GetAsync(string user, [FromBody] EventsFilterInput? eventsFilter = null)
        {
			eventsFilter ??= new EventsFilterInput();
            var events = await _eventService.EventsAsync(user, eventsFilter);
            return Ok(events);
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountAsync(string user)
        {
            var eventsCount = await _eventService.EventsCountAsync(user);
            return Ok(eventsCount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string user, EventInput eventInput)
        {
            var attendeeInput = new AttendeeInput
            {
                Email = AuthenticatedUserEmail!,
                Response = AttendeeResponse.Accepted
            };
            var @event = await _eventService.CreateAsync(user, eventInput, attendeeInput);
            return Ok(@event);
        }

        [HttpPatch("{eventId}")]
        public async Task<IActionResult> UpdateAsync(string eventId, string user, EventInput eventInput)
        {
            var @event = await _eventService.UpdateAsync(user, eventId, eventInput);
            return Ok(@event);
        }

        [HttpPatch("{eventId}/reply")]
        public async Task<IActionResult> ReplyAsync(string eventId, string user, AttendeeInput attendeeInput)
        {
            attendeeInput.Email = AuthenticatedUserEmail!;
            var @event = await _eventService.ReplyAsync(user, eventId, attendeeInput);
            return Ok(@event);
        }
    }
}