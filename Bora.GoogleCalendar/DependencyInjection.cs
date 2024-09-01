using Bora.Accounts;
using Bora.Events;
using Bora.GoogleCalendar;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGoogleCalendarService(this IServiceCollection serviceCollection, GoogleCalendarConfiguration googleCalendarConfiguration)
        {
            serviceCollection.AddScoped<IAccountDataStore, AccountDataStore>();
            serviceCollection.AddSingleton(googleCalendarConfiguration);
            serviceCollection.AddScoped<IEventService, GoogleCalendarService>();
            return serviceCollection;
        }
    }
}
