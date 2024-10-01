using Bora.Wheather;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public class WheatherServiceTests : TestsBase
    {
        [Fact]
        public async Task GetForecastAsync()
        {
            //when
            var wheatherService = _serviceProvider.GetService<IWheatherService>()!;
            var forecast = await wheatherService.GetForecastAsync();
        }
    }
}