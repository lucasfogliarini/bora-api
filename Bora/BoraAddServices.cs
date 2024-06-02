using Bora.Accounts;
using Bora.Contents;
using Bora.Events;
using Bora.Scenarios;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BoraAddServices
    {
        /// <summary>
        /// Add Bora Services
        /// </summary>
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEventService, EventService>();
            serviceCollection.AddScoped<IAccountService, AccountService>();
            serviceCollection.AddScoped<IScenarioService, ScenarioService>();
            serviceCollection.AddScoped<IContentService, ContentService>();
            serviceCollection.AddScoped<IAccountDataStore, AccountDataStore>();
            return serviceCollection;
        }
    }
}
