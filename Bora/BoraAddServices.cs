using Bora.Accounts;
using Bora.Contents;
using Bora.Events;
using Bora.Scenarios;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BoraAddServices
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEventService, EventService>();
            serviceCollection.AddTransient<IAccountService, AccountService>();
            serviceCollection.AddTransient<IScenarioService, ScenarioService>();
            serviceCollection.AddTransient<IContentService, ContentService>();
            serviceCollection.AddTransient<IAccountDataStore, AccountDataStore>();
        }

        public static void AddBoraAzureTablesRepository(this IServiceCollection serviceCollection, string storageConnectionString)
        {
            serviceCollection.AddAzureTablesRepository(storageConnectionString);
        }
    }
}
