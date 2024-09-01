using Bora.Accounts;
using Bora.Contents;
using Bora.Events;
using Bora.JsonWebToken;
using Bora.Scenarios;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAccountService, AccountService>();
            serviceCollection.AddScoped<IScenarioService, ScenarioService>();
            serviceCollection.AddScoped<IContentService, ContentService>();
            return serviceCollection;
        }

        public static IServiceCollection AddJwtService(this IServiceCollection serviceCollection, JwtConfiguration jwtConfiguration)
        {
            serviceCollection.AddScoped<IJwtService, JwtService>();
            serviceCollection.AddSingleton(jwtConfiguration);
            return serviceCollection;
        }
    }
}
