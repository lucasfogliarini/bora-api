using Bora.Authentication.JsonWebToken;
using Bora.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public abstract class TestsBase
    {
        protected readonly ServiceProvider _serviceProvider;
        protected const string ARQUITETO_EMAIL = "lucasfogliarini@gmail.com";
        public TestsBase()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Define o diretório base para buscar o arquivo JSON
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            AddConfigs(configuration, serviceCollection);

            serviceCollection.AddBoraServices();
            var boraRepositoryConnectionStringKey = "ConnectionStrings:BoraRepository";
            var connectionString = configuration[boraRepositoryConnectionStringKey];
            serviceCollection.AddEFCoreRepository(EFCoreProvider.InMemory, connectionString);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider.GetService<IRepository>()!.SeedAsync().Wait();
        }

        private static void AddConfigs(IConfigurationRoot configuration, ServiceCollection serviceCollection)
        {
            var googleCalendarSection = configuration.GetSection(GoogleCalendarConfiguration.GoogleCalendarSection);
            var googleCalendarConfig = googleCalendarSection.Get<GoogleCalendarConfiguration>();
            serviceCollection.AddSingleton(googleCalendarConfig!);

            var jwtSection = configuration.GetSection(JwtConfiguration.JwtSection);
            var jwtConfiguration = jwtSection.Get<JwtConfiguration>();
            serviceCollection.AddSingleton(jwtConfiguration!);
        }
    }
}