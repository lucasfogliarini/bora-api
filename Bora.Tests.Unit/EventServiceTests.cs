using Bora.Events;
using Google.Apis.Calendar.v3.Data;

namespace Bora.Tests.Unit
{
    public class EventServiceTests
    {
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
        public void GetWhatsAppChat(string description, string expectedWhatsAppChat)
        {
            var @event = new Event
            {
                Description = description
            };
            var whatsAppChatUrl = EventService.GetWhatsAppChat(@event);

            Assert.Equal(expectedWhatsAppChat, whatsAppChatUrl);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("https://anyurl.com", null)]
        [InlineData("adfhudsha https://discord.gg/GR7g82QB5U hasufhsdauf", "https://discord.gg/GR7g82QB5U")]
        public void GetDiscordChannel(string description, string expectedDiscordChannel)
        {
            var @event = new Event
            {
                Description = description
            };
            var discordChannel = EventService.GetDiscordChannel(@event);

            Assert.Equal(expectedDiscordChannel, discordChannel);
        }
    }
}