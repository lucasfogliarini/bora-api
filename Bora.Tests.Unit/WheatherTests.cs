using Bora.Wheather;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Tests.Unit
{
    public class WheatherTests : TestsBase
    {
        [Fact]
        public async Task GetForecastAsync()
        {
            //when
            var wheather = _serviceProvider.GetService<IWheather>()!;
            var forecast = await wheather.GetForecastAsync();
        }
    }
}