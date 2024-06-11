using Bora.Accounts;
using Bora.Entities;
using Google.Apis.Calendar.v3;
using Google.Apis.Tasks.v1;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SendUpdatesEnum = Google.Apis.Calendar.v3.EventsResource.PatchRequest.SendUpdatesEnum;

namespace Bora.Events
{
    public class EventService : IEventService
    {
        CalendarService _calendarService;
        TasksService _tasksService;
		private readonly IRepository _boraRepository;
		private readonly IAccountDataStore _accountDataStore;
        private readonly IAccountService _accountService;
        //private IEnumerable<Account> _accounts;
        //private IEnumerable<Account> Accounts => _accounts ??= _boraRepository.All<Account>();//for azure tables
        const string BORA_ADMIN_EMAIL = "bora.reunir@gmail.com";
        const string BORA_ALWAYS_PRESENT_EMAIL = "lucasfogliarini@gmail.com";

        public EventService(IRepository boraRepository, IAccountService accountService, IAccountDataStore accountDataStore)
        {
            _boraRepository = boraRepository;
            _accountDataStore = accountDataStore;
            _accountService = accountService;
        }

        public async Task<IEnumerable<EventOutput>> EventsAsync(string user, EventsFilterInput eventsFilter)
        {
            await InitializeCalendarServiceAsync(user);
            var request = _calendarService.Events.List(eventsFilter.CalendarId);
            request.TimeMinDateTimeOffset = eventsFilter.TimeMin ?? DateTime.Now;
            request.TimeMaxDateTimeOffset = eventsFilter.TimeMax ?? DateTime.Now.AddMonths(6);
            request.Q = eventsFilter.Query;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            var eventsCount = eventsFilter.FavoritesCount ? await EventsCountAsync(user) : null;
            var events = await request.ExecuteAsync();

            var eventItems = events.Items.AsEnumerable();
            AlwaysPresent(eventItems);

            var account = _accountService.GetAccountByUsername(user);
            if (account.OnlySelfOrganizer)
                eventItems = eventItems.Where(i => i.Organizer.Self == account.OnlySelfOrganizer);

			var eventsOutput = eventItems.Where(i => i.Visibility == "public").Select(i=>ToEventOutput(i, eventsCount));
            if(eventsFilter.HasTicket.GetValueOrDefault())
                eventsOutput = eventsOutput.Where(e => e.TicketUrl != null);
            return eventsOutput;
        }
		public async Task<EventsCountOutput> EventsCountAsync(string user)
        {
            await InitializeCalendarServiceAsync(user);
            var request = _calendarService.Events.List("primary");
            request.TimeMax = DateTime.Now;
            request.SingleEvents = true;
            request.MaxResults = 5000;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            var events = await request.ExecuteAsync();
            var eventItems = events.Items.Where(i => i.Visibility == "public");
            var eventsCount = new EventsCountOutput
            {
                ClosestAttendees = ClosestAttendees(eventItems),
                FavoriteLocations = FavoriteLocations(eventItems)
            };
            return eventsCount;
        }
        public async Task<EventOutput> CreateAsync(string user, EventInput eventInput, AttendeeInput attendeeInput)
        {
            ValidateEvent(eventInput);
            await InitializeCalendarServiceAsync(user);
            eventInput.Create = true;
            var @event = ToGoogleEvent(eventInput);
            var request = _calendarService.Events.Insert(@event, eventInput.CalendarId);
            request.ConferenceDataVersion = 1;
            var gEvent = await request.ExecuteAsync();
            CreateReminderTask(eventInput);
            await AddOrUpdateAttendeeAsync(gEvent, attendeeInput);
            return ToEventOutput(gEvent);
        }
        public async Task<EventOutput> UpdateAsync(string user, string eventId, EventInput eventInput)
        {
            ValidateEvent(eventInput);
            await InitializeCalendarServiceAsync(user);
            var @event = ToGoogleEvent(eventInput);
            var request = _calendarService.Events.Patch(@event, eventInput.CalendarId, eventId);
            request.SendUpdates = SendUpdates(@event);
            var gEvent = await request.ExecuteAsync();
            return ToEventOutput(gEvent);
        }
        public async Task<EventOutput> ReplyAsync(string user, string eventId, AttendeeInput attendeeInput)
        {
            await InitializeCalendarServiceAsync(user);

            var @event = await GetAsync(eventId) ?? throw new ValidationException($"Não existe um evento com id {eventId}.");
            await AddOrUpdateAttendeeAsync(@event, attendeeInput);

            return ToEventOutput(@event);
        }
        public static string[]? GetAttachments(Event @event)
        {
            if (@event?.Attachments != null)
            {
                return @event.Attachments.Select(e => $"https://drive.google.com/uc?id={e.FileId}").ToArray();
            }
            return null;
        }
        public static string? GetTicketUrl(Event @event)
        {
            var ticketDomains = new[] { "sympla", "eleventickets", "uhuu", "eventbrite", "ingresse", "ticketswap", "vamoapp", "ingressorapido", "uhuu", "eventbrite", "lets.events", "appticket", "ingressonacional", "minhaentrada", "eventim" };
            return GetUrl(@event.Description, ticketDomains);
        }
        public static string? GetTicketDomain(Event @event)
        {
            if (@event?.Description != null)
            {
                var urlPattern = @"(?:https?:\/\/)?(?:www\.)?([^\/]+)";
                var matches = Regex.Matches(@event.Description, urlPattern);
                return matches?.ElementAtOrDefault(1)?.Value;
            }
            return null;
        }
        public static string? GetSpotifyUrl(Event @event)
        {
            return GetUrl(@event.Description, "spotify");
        }
        public static string? GetInstagramUrl(Event @event)
        {
            return GetUrl(@event.Description, "instagram");
        }
        public static string? GetYouTubeUrl(Event @event)
        {
            return GetUrl(@event.Description, "youtube", "youtu.be");
        }
        public static string? GetWhatsAppGroupChat(Event @event)
        {
            return GetUrl(@event.Description, "chat.whatsapp.com");
        }
        public static string? GetSpotifyJam(Event @event)
        {
            return GetUrl(@event.Description, "spotify.link", "open.spotify.com");
        }
        public static string? GetDiscordChannel(Event @event)
        {
            bool hasDiscord = !string.IsNullOrEmpty(@event.Location) && @event.Location.Contains("discord.gg");
            return hasDiscord ? GetUrl(@event.Location, "discord.gg") : null;
        }
        public static string? GetWhatsAppMe(Event @event)
        {
            bool hasWaMe = !string.IsNullOrEmpty(@event.Location) && @event.Location.Contains("wa.me");
            return hasWaMe ? GetUrl(@event.Location, "wa.me") : null;
        }
        public static string GetConferenceUrl(Event @event)
        {
            var conferenceUrl = GetDiscordChannel(@event) ?? GetWhatsAppMe(@event) ?? GetSpotifyJam(@event) ?? @event.HangoutLink;
            return conferenceUrl;
        }
        public static decimal GetDiscount(Event @event)
        {
            if (@event.Summary != null)
            {
                Regex regex = new(@"\b(\d{1,3})%\$");
                Match match = regex.Match(@event.Summary);

                if (match.Success)
                    return decimal.Parse(match.Groups[1].Value);
            }
            return 0;
        }

        private static void ValidateEvent(EventInput eventInput)
        {
            if (eventInput.Start.HasValue && eventInput.Start.Value < DateTime.Now)
                throw new ValidationException("O encontro precisa ser maior que agora ...");
        }
        private async Task AddOrUpdateAttendeeAsync(Event @event, AttendeeInput attendeeInput)
        {
            if(@event.Creator?.Email == null || @event.Id == null)
                throw new ValidationException($"O evento deve ter um Creator e um Id para adicionar um novo Attendee.");

            @event.Attendees ??=
            [
                new EventAttendee
                {
                    Email = @event.Creator.Email,
                    ResponseStatus = AttendeeResponseExtensions.ToResponseStatus(attendeeInput.Response),
                    Organizer = true
                }
            ];
            var eventAttendee = @event.Attendees.FirstOrDefault(e => e.Email == attendeeInput.Email);
            if (eventAttendee == null)
            {
                eventAttendee = new EventAttendee
                {
                    Email = attendeeInput.Email,
                };
                @event.Attendees.Add(eventAttendee);
            }
            eventAttendee.ResponseStatus =  attendeeInput.Response.ToResponseStatus() ?? eventAttendee.ResponseStatus;
            eventAttendee.Comment = attendeeInput.Comment ?? eventAttendee.Comment;

            @event.GuestsCanModify = true; // @event.Attendees.Count <= 2;

            var request = _calendarService.Events.Patch(@event, @event.Organizer.Email, @event.Id);
            request.SendUpdates = SendUpdates(@event);
            await request.ExecuteAsync();
        }
        private IEnumerable<AttendeeOutput>? GetAttendees(Event @event)
        {
            if (@event.Attendees != null)
            {
                var emails = @event.Attendees.Select(e => e.Email);
				var attendeeAccounts = _boraRepository.Where<Account>(e => emails.Contains(e.Email));
                var attendeeOutputs = attendeeAccounts.Select(e => new AttendeeOutput
                {
                    Email = e.Email,
                    Name = e.Name,
                    Username = e.Username,
                    Photo = e.Photo,
                    Instagram = e.Instagram,
                    WhatsApp = e.WhatsApp,
                    Spotify = e.Spotify,
                    IsPartner = e.IsPartner && e.CalendarAuthorized
                }).ToList();
                var attendeesWithComment = @event.Attendees.Where(e => e.Comment != null);
                foreach (var attendee in attendeesWithComment)
                {
                    var attendeeOutput = attendeeOutputs.FirstOrDefault(e => e.Email == attendee.Email);
                    if (attendeeOutput != null)
                        attendeeOutput.Comment = attendee.Comment;
                }

                return attendeeOutputs;
            }
            return null;
        }
        private void OrderAttendeesByProximityRate(IEnumerable<AttendeeOutput> attendeeOutputs, EventsCountOutput? eventsCountOutput)
        {
            if (eventsCountOutput?.ClosestAttendees != null)
            {
                foreach (var attendee in attendeeOutputs)
                {
                    attendee.ProximityRate = GetProximityRate(attendee.Email, eventsCountOutput!.ClosestAttendees);
                };
                attendeeOutputs = attendeeOutputs.OrderByDescending(e => e.ProximityRate);
            }
        }
        private static decimal? GetProximityRate(string email, IEnumerable<ClosestAttendee> closestAttendees)
        {
            var closestAttendee = closestAttendees?.FirstOrDefault(e => e.Email == email);
            return closestAttendee == null ? 0 : closestAttendee.ProximityRate;
        }
        private IEnumerable<ClosestAttendee> ClosestAttendees(IEnumerable<Event> eventItems)
        {
            var eventsCount = eventItems.Where(i => i.Attendees != null && i.Attendees.Count(e=>e.ResponseStatus == "accepted") > 1);
            var closestAttendees = eventsCount.SelectMany(p => p.Attendees)
                                     .GroupBy(e => e.Email)
                                     .Select(e => new ClosestAttendee { Email = e.Key, AttendeeCount = e.Count(), EventsCount = eventsCount.Count() })
                                     .OrderByDescending(e => e.AttendeeCount);
            return closestAttendees;
        }
        private IEnumerable<FavoriteLocation> FavoriteLocations(IEnumerable<Event> eventItems)
        {
            var favoriteLocations = eventItems.Where(e => e.Location != null)
                                    .GroupBy(e => e.Location)
                                    .Select(e => new FavoriteLocation { Location = e.Key, Count = e.Count() })
                                    .OrderByDescending(e => e.Count);
            return favoriteLocations;
        }
        private static SendUpdatesEnum SendUpdates(Event @event)
        {
            if (@event.Attendees != null)
            {
                return SendUpdatesEnum.All;
            }
            return SendUpdatesEnum.None;
        }
        private async Task<Event> GetAsync(string eventId)
        {
            var request = _calendarService.Events.Get("primary", eventId);
            var gEvent = await request.ExecuteAsync();
            return gEvent;
        }
        private async Task InitializeCalendarServiceAsync(string username)
        {
            var userCredential = await _accountDataStore.GetUserCredentialAsync(username);

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
            });

			_tasksService = new TasksService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = userCredential
			});
		}
        private async Task CreateReminderTask(EventInput eventInput)
        {
            if (eventInput.CreateReminderTask)
            {
                var taskList = "MDExMTAxNTQ4OTU1MzE4NzMxMDI6MDow";//Tarefas
                var reminderTask = new Google.Apis.Tasks.v1.Data.Task
                {
                    Due = DateTime.Now.ToString("O"),
                    Title = $"{eventInput.Title} - {eventInput.Start.Value}"
                };
                await _tasksService.Tasks.Insert(reminderTask, taskList).ExecuteAsync();
            }
        }
        private static Event ToGoogleEvent(EventInput eventInput)
        {
            if (eventInput.Create && eventInput.Start == null)
                eventInput.Start = DateTimeOffset.Now.AddMinutes(10);

            if (eventInput.Start != null && eventInput.End == null)
                eventInput.End = eventInput.Start.Value.AddMinutes(30);

            var @event = new Event
            {
                Location = eventInput.Location,
                Summary = eventInput.Title,
                Description = eventInput.Description,
                Start = new EventDateTime { DateTimeDateTimeOffset = eventInput.Start },
                End = new EventDateTime { DateTimeDateTimeOffset = eventInput.End },
            };

            if (eventInput.Create && (eventInput.Start == null || eventInput.End == null))
            {
                throw new ValidationException("Start and end times must either not null.");
            }

            if (eventInput.Color.HasValue)
			{
                @event.ColorId = ((int)eventInput.Color).ToString();
			}

            if (eventInput.Public != null)
            {
                @event.Visibility = eventInput.Public.Value ? "public" : "private";
            }			

			if (eventInput.AddConference)
            {
                AddGoogleMeet(@event);
            }

            return @event;
        }
        private static void AddGoogleMeet(Event @event)
        {
            @event.ConferenceData = new ConferenceData
            {
                CreateRequest = new CreateConferenceRequest
                {
                    RequestId = $"{@event.Summary}-{@event.Start?.DateTime}",
                    ConferenceSolutionKey = new ConferenceSolutionKey
                    {
                        Type = "hangoutsMeet"
                    }
                }
            };
        }
        private EventOutput ToEventOutput(Event @event, EventsCountOutput? eventsCountOutput = null)
        {
            IEnumerable<AttendeeOutput> attendeeOutputs = GetAttendees(@event);
            if (attendeeOutputs != null)
            {
                OrderAttendeesByProximityRate(attendeeOutputs, eventsCountOutput);

                attendeeOutputs = attendeeOutputs.OrderByDescending(e => e.IsPartner);
            }

            return new EventOutput
            {
                Id = @event.Id,  
                Title = @event.Summary,
                Description = @event.Description,
                Location = @event.Location,
                Start = @event.Start.DateTime ?? DateTime.Parse(@event.Start.Date),
                End = @event.End.DateTime ?? DateTime.Parse(@event.End.Date),
                GoogleEventUrl = @event.HtmlLink,
                Public = @event.Visibility == "public",
                Chat = GetWhatsAppGroupChat(@event),
                ConferenceUrl = GetConferenceUrl(@event),
                Attendees = attendeeOutputs,
                TicketUrl = GetTicketUrl(@event),
                TicketDomain = GetTicketDomain(@event),
                SpotifyUrl = GetSpotifyUrl(@event),
                Discount = GetDiscount(@event),
                InstagramUrl = GetInstagramUrl(@event),
                YouTubeUrl = GetYouTubeUrl(@event),
                Attachments = GetAttachments(@event),
                Deadline = GetDeadLine(@event),
                Recurrence = @event.Recurrence,
            };
        }
        private static string? GetUrl(string description, params string[] domains)
        {
            if (description != null)
            {
                var urlPattern = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
                var matches = Regex.Matches(description, urlPattern);
                foreach (Match match in matches.Where(m=>m.Success))
                {
                    bool urlHasDomain = domains.Any(domain => match.Value.Contains(domain));
                    if (urlHasDomain)
                        return match.Value;
                }
            }
            return null;
        }
        private static DateTime? GetDeadLine(Event @event)
        {
            var reminderMinutes = @event.Reminders.Overrides?.FirstOrDefault()?.Minutes;
            if (reminderMinutes != null && @event.Start.DateTime.HasValue)
            {
                return @event.Start.DateTime.Value.AddMinutes(-reminderMinutes.Value);
            }
            return null;
        }
        private async Task AlwaysPresent(IEnumerable<Event> eventItems)
        {
            var boraAloneEvents = eventItems
                                .Where(e => e.Organizer.Email == BORA_ADMIN_EMAIL
                                    && e.Attendees != null
                                    && !e.Attendees.Any(a => a.Email == BORA_ALWAYS_PRESENT_EMAIL))
                                .ToList();

            if (boraAloneEvents.Any())
            {
                var alwaysPresentAttendee = new AttendeeInput
                {
                    Email = BORA_ALWAYS_PRESENT_EMAIL,
                    Response = AttendeeResponse.Accepted,
                    Comment = "Bora!"
                };

                await Task.WhenAll(boraAloneEvents.Select(async e => await AddOrUpdateAttendeeAsync(e, alwaysPresentAttendee)));
            }
        }
    }
}
