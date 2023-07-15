using Google.Apis.Auth.OAuth2;

namespace Bora.Events
{
    public class GoogleCalendarConfiguration
    {
        public const string AppSettingsKey = "GoogleCalendar";
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TokenFolder { get; set; }
        public string? ApplicationName { get; set; }
        public ClientSecrets GoogleClientSecrets()
        {
            return new ClientSecrets
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret
            };
        }
    }
}
