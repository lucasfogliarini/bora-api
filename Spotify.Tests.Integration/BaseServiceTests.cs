using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spotify.Tests.Integration
{
    public abstract class BaseServiceTests
    {
        protected readonly ServiceProvider _serviceProvider;

        public BaseServiceTests()
        {
            var serviceCollection = new ServiceCollection();
            var iConfiguration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            serviceCollection.AddSingleton(iConfiguration);
            serviceCollection.AddSpotifyService();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}