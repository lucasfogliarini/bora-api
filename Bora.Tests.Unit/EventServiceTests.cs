using Bora.Events;
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
        [InlineData(null, null, typeof(ValidationException), "Usu�rio n�o existe.")]
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
        [InlineData(null, null, typeof(ValidationException), "Usu�rio n�o existe.")]
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
        [InlineData(null, null, null)]
        [InlineData("https://anyurl.com", null, null)]
        [InlineData("aa https://www.sympla.com.br/evento/1 bb", "https://www.sympla.com.br/evento/1", "www.sympla.com.br")]
        [InlineData("aa https://uhuu.com/evento/evento1 bb", "https://uhuu.com/evento/evento1", "uhuu.com")]
        [InlineData("aa https://www.eventbrite.com.br/e/evento1 bb", "https://www.eventbrite.com.br/e/evento1", "www.eventbrite.com.br")]
        [InlineData("aa https://minhaentrada.com.br/evento/evento1 bb", "https://minhaentrada.com.br/evento/evento1", "minhaentrada.com.br")]
        [InlineData("aa https://www.ingressonacional.com.br/evento/1 bb", "https://www.ingressonacional.com.br/evento/1", "www.ingressonacional.com.br")]
        [InlineData("aa https://www.ingressorapido.com.br/event/34693-1/d/74625 bb", "https://www.ingressorapido.com.br/event/34693-1/d/74625", "www.ingressorapido.com.br")]
        [InlineData("aa https://vamoapp.com/events/11706/1 bb", "https://vamoapp.com/events/11706/1", "vamoapp.com")]
        [InlineData("aa https://lets.events/e/evento1 bb", "https://lets.events/e/evento1", "lets.events")]
        [InlineData("aa https://appticket.com.br/evento1 bb", "https://appticket.com.br/evento1", "appticket.com.br")]
        [InlineData("aa https://www.ticketswap.com.br/event/evento1 bb", "https://www.ticketswap.com.br/event/evento1", "www.ticketswap.com.br")]
        [InlineData("aa https://www.ingresse.com/evento1 bb", "https://www.ingresse.com/evento1", "www.ingresse.com")]
        [InlineData("aa https://www.eventim.com.br/event/evento1 bb", "https://www.eventim.com.br/event/evento1", "www.eventim.com.br")]
        [InlineData("aa https://eleventickets.com/produto/valenbar/88/sabado-com-o-valen-e-mais-gostoso bb", "https://eleventickets.com/produto/valenbar/88/sabado-com-o-valen-e-mais-gostoso", "eleventickets.com")]
        public void GetTicketUrl(string description, string expectedTicketUrl, string expectedTicketDomain)
        {
            var @event = new Event
            {
                Description = description
            };
            var ticketUrl = EventService.GetTicketUrl(@event);
            var ticketDomain = EventService.GetTicketDomain(@event);

            Assert.Equal(expectedTicketUrl, ticketUrl);
            Assert.Equal(expectedTicketDomain, ticketDomain);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("https://anyurl.com", null)]
        [InlineData("https://www.ingresse.com/evento1 https://open.spotify.com/playlist/25GJEl3ZMUla1NWO9YvXvt", "https://open.spotify.com/playlist/25GJEl3ZMUla1NWO9YvXvt")]
        [InlineData("https://www.ingresse.com/evento1 https://open.spotify.com/track/6e0EbCex8C9LVogl3Qhogn", "https://open.spotify.com/track/6e0EbCex8C9LVogl3Qhogn")]
        [InlineData("https://open.spotify.com/playlist/25GJEl3ZMUla1NWO9YvXvt https://www.ingresse.com/evento1 ", "https://open.spotify.com/playlist/25GJEl3ZMUla1NWO9YvXvt")]
        public void GetSpotifyUrl(string description, string expectedSpotifyUrl)
        {
            var @event = new Event
            {
                Description = description
            };
            var spotifyUrl = EventService.GetSpotifyUrl(@event);

            Assert.Equal(expectedSpotifyUrl, spotifyUrl);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("https://anyurl.com", null)]
        [InlineData("https://www.ingresse.com/evento1 https://www.instagram.com/p/CKYy1AbBl30aB1IHY6T7R4dkGOHURHAgPismig0/", "https://www.instagram.com/p/CKYy1AbBl30aB1IHY6T7R4dkGOHURHAgPismig0/")]
        [InlineData("https://www.instagram.com/p/CKYy1AbBl30aB1IHY6T7R4dkGOHURHAgPismig0/ https://www.ingresse.com/evento1 ", "https://www.instagram.com/p/CKYy1AbBl30aB1IHY6T7R4dkGOHURHAgPismig0/")]
        public void GetInstagramUrl(string description, string expectedInstagramUrl)
        {
            var @event = new Event
            {
                Description = description
            };
            var instagramUrl = EventService.GetInstagramUrl(@event);

            Assert.Equal(expectedInstagramUrl, instagramUrl);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("https://anyurl.com", null)]
        [InlineData("https://www.ingresse.com/evento1 https://www.youtube.com/watch?v=video1", "https://www.youtube.com/watch?v=video1")]
        [InlineData("https://www.ingresse.com/evento1 https://youtu.be/video1", "https://youtu.be/video1")]
        [InlineData("https://www.youtube.com/watch?v=video1 https://www.ingresse.com/evento1 ", "https://www.youtube.com/watch?v=video1")]
        public void GetYouTubeUrl(string description, string expectedYouTubeUrl)
        {
            var @event = new Event
            {
                Description = description
            };
            var youTubeUrl = EventService.GetYouTubeUrl(@event);

            Assert.Equal(expectedYouTubeUrl, youTubeUrl);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("1RW53sc7b55j_5hrlW1365ftgg7Tr26K1", new[]{ "https://drive.google.com/uc?id=1RW53sc7b55j_5hrlW1365ftgg7Tr26K1" })]
        public void GetAttachments(string eventAttachmentId, string[] expectedAttachments)
        {
            var @event = eventAttachmentId == null ? null : new Event
            {
                Attachments = new List<EventAttachment>
                {
                    new EventAttachment
                    {
                        FileId = eventAttachmentId
                    }
                }
            };
            var attachments = EventService.GetAttachments(@event);

            Assert.Equal(expectedAttachments, attachments);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("https://anyurl.com", null)]
        [InlineData("adfhudsha https://chat.whatsapp.com/BVoLGzZTQYKFpCyzUYJnPu hasufhsdauf", "https://chat.whatsapp.com/BVoLGzZTQYKFpCyzUYJnPu")]
        public void GetWhatsAppGroupChat(string description, string expectedWhatsAppChat)
        {
            var @event = new Event
            {
                Description = description
            };
            var whatsAppChatUrl = EventService.GetWhatsAppGroupChat(@event);

            Assert.Equal(expectedWhatsAppChat, whatsAppChatUrl);
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData("https://anyurl.com", null, null, null)]
        [InlineData(" https://horizon.meta.com/lucasfogliarini ", "https://meet.google.com/gpp-qisy-hpi", "", "https://horizon.meta.com/lucasfogliarini")]
        [InlineData(" https://discord.gg/GR7g82QB5U ", "https://meet.google.com/gpp-qisy-hpi", "", "https://discord.gg/GR7g82QB5U")]
        [InlineData(" https://wa.me/51992364249 ", "https://meet.google.com/gpp-qisy-hpi", "", "https://wa.me/51992364249")]
        [InlineData("location", "https://meet.google.com/gpp-qisy-hpi", "", "https://meet.google.com/gpp-qisy-hpi")]
        [InlineData("location","hangoutLink", "aa https://spotify.link/DeJLQwssWJb aa", "https://spotify.link/DeJLQwssWJb")]
        [InlineData("location", "hangoutLink", "aa https://open.spotify.com/playlist/07BYjFAmOPfj8kLdVbMgfK?si=85272cfd42d34d4e aa", "https://open.spotify.com/playlist/07BYjFAmOPfj8kLdVbMgfK?si=85272cfd42d34d4e")]
        public void GetConferenceUrl(string location, string hangoutLink, string? description, string expectedConferenceUrl)
        {
            var @event = new Event
            {
                Location = location,
                HangoutLink = hangoutLink,
                Description = description
            };
            var conferenceUrl = EventService.GetConferenceUrl(@event);

            Assert.Equal(expectedConferenceUrl, conferenceUrl);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("sem desconto", 0)]
        [InlineData("titulo tal com 20%$", 20)]
        public void GetDiscount(string summary, decimal expectedDiscount)
        {
            var @event = new Event
            {
                 Summary = summary
            };
            var discount = EventService.GetDiscount(@event);

            Assert.Equal(expectedDiscount, discount);
        }
    }
}