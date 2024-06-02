using Bora.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public abstract class TestsBase
    {
        protected readonly ServiceProvider _serviceProvider;
        protected const string ADMIN_EMAIL = "lucasfogliarini@gmail.com";
        public TestsBase()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Define o diretório base para buscar o arquivo JSON
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var googleCalendarSection = configuration.GetSection(GoogleCalendarConfiguration.AppSettingsKey);
            var googleCalendarConfig = googleCalendarSection.Get<GoogleCalendarConfiguration>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(googleCalendarConfig!);
            serviceCollection.AddServices();
            var boraRepositoryConnectionStringKey = "ConnectionStrings:BoraRepository";
            var connectionString = configuration[boraRepositoryConnectionStringKey];
            serviceCollection.AddEFCoreRepository(EFCoreProvider.SqlServer, connectionString);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider.GetService<IRepository>()!.SeedAsync().Wait();
        }
    }
}