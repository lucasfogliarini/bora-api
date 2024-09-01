using Bora.Accounts;
using Bora.Contents;
using Bora.Scenarios;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBoraServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAccountService, AccountService>();
            serviceCollection.AddScoped<IScenarioService, ScenarioService>();
            serviceCollection.AddScoped<IContentService, ContentService>();
            return serviceCollection;
        }
    }
}
