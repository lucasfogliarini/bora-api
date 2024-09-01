using Bora.Events;
using Bora.GoogleCalendar;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Bora.Tests.Unit
{
    public class EventServiceTests : TestsBase
    {
        [Theory]
        [InlineData("bora.work")]
        public async void EventsAsync(string? user)
        {
            var eventsFilterInput = new EventsFilterInput
            {
                //FavoritesCount = true,
                //HasTicket = true,
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var events = await eventService.EventsAsync(user, eventsFilterInput);

            Assert.True(true);//apenas pra debugar
        }

        [Theory]
        [InlineData("lucasfogliarini", EventOutput.PRIVATE, false)]
        [InlineData("lucasfogliarini", EventOutput.PRIVADO, false)]
        [InlineData("lucasfogliarini", "", true)]
        [InlineData("lucasfogliarini", null, true)]
        public async void EventsAsync_ShouldPublicOrPrivate_When(string user, string description, bool expectedPublic)
        {
            var eventsFilterInput = new EventsFilterInput
            {
                TimeMax = DateTime.Now.AddDays(5)
                //FavoritesCount = true,
                //HasTicket = true,
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var events = await eventService.EventsAsync(user, eventsFilterInput);
            events = events.Where(e=>!string.IsNullOrWhiteSpace(e.Description));

            var eventFiltered = string.IsNullOrWhiteSpace(description) ?
                                events.FirstOrDefault(e=> !new[] { EventOutput.PRIVADO, EventOutput.PRIVATE }.Any(pvt => e.Description.Contains(pvt))) :
                                events.FirstOrDefault(e => e.Description!.Contains(description));
            if (eventFiltered == null)
            {
                Assert.True(true, "Evento não encontrado para o teste");
                return;
            }
                
            Assert.Equal(expectedPublic, eventFiltered.Public);
        }

        [Theory]
        [InlineData("bora.work", true)]
        [InlineData("bora.work", false)]
        [InlineData("bora.work", null)]
        public async void EventsAsync_ShouldFilter_Tickets(string? user, bool? hasTicket)
        {
            var eventsFilterInput = new EventsFilterInput
            {
                HasTicket = hasTicket,
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var events = await eventService.EventsAsync(user, eventsFilterInput);

            var allHasTickets = events.All(e => e.TicketUrl != null);
            Assert.Equal(hasTicket ?? false, allHasTickets);
        }

        [Theory]
        [InlineData(null, null, typeof(ValidationException), "Usuário não existe.")]
        [InlineData("bora.work", -1, typeof(ValidationException), "O encontro precisa ser maior que agora ...")]
        [InlineData("bora.work", null, null, "Start and end times must either not null.")]
        [InlineData("bora.work", 1, null, null)]
        public async void CreateAsync(string? user, int? startAddMinutes, Type? exactExceptionType, string? expectedMessage)
        {
            DateTimeOffset? startDateTimeOffset = startAddMinutes == null ? null : DateTimeOffset.Now.AddMinutes(startAddMinutes.Value);
            var eventInput = new EventInput
            {
                Public = false,
                Start = startDateTimeOffset,
                End = startDateTimeOffset
            };

            var attendeeInput = new AttendeeInput
            {
                Email = ARQUITETO_EMAIL,
                Response = AttendeeResponse.accepted
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var actualException = await Record.ExceptionAsync(async () =>
            {
                await eventService.CreateAsync(user, eventInput, attendeeInput);
            });

            if(exactExceptionType == null)//when works
            {
                Assert.Null(actualException);
            }
            else
            {
                Assert.IsType(exactExceptionType, actualException);
                Assert.Equal(expectedMessage, actualException.Message);
            }
        }

        [Theory]
        [InlineData(null, null, typeof(ValidationException), "Usuário não existe.")]
        [InlineData("bora.work", -1, typeof(ValidationException), "O encontro precisa ser maior que agora ...")]
        [InlineData("bora.work", null, null, null)]
        [InlineData("bora.work", 1, null, null)]
        public async void UpdateAsync(string? user, int? startAddDays, Type? exactExceptionType, string? expectedMessage)
        {
            DateTimeOffset? startDateTimeOffset = startAddDays == null ? null : DateTimeOffset.Now.AddDays(startAddDays.Value);
            string? eventId = "nqt67u3429s96rei4njaiuu1p8";
            var eventInput = new EventInput
            {
                Title = $"{nameof(UpdateAsync)} test",
                Public = false,
                Start = startDateTimeOffset,
                End = null
            };

            var attendeeInput = new AttendeeInput
            {
                Email = ARQUITETO_EMAIL,
                Response = AttendeeResponse.accepted
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var actualException = await Record.ExceptionAsync(async () =>
            {
                await eventService.UpdateAsync(user, eventId, eventInput);
            });

            if (exactExceptionType == null)
            {
                Assert.Null(actualException);
            }
            else
            {
                Assert.IsType(exactExceptionType, actualException);
                Assert.Equal(expectedMessage, actualException.Message);
            }
        }

        [Theory]
        [InlineData("lucasfogliarini", "cqv67d4ckva8e6ks4smulescuk", true)]//precisa buscar o evento pra tirar o #private
        [InlineData("lucasfogliarini", "cqv67d4ckva8e6ks4smulescuk", false)]
        public async void UpdateAsync_ShouldPublicOrPrivate_When(string user, string eventId, bool expectedPublic)
        {
            var eventInput = new EventInput
            {
                Public = expectedPublic,
            };

            var eventService = _serviceProvider.GetService<IEventService>()!;

            var @event = await eventService.UpdateAsync(user, eventId, eventInput);

            Assert.Equal(expectedPublic, @event.Public);
        }
    }
}