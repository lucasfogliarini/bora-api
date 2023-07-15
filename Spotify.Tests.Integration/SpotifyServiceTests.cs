using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Spotify.Tests.Integration
{
    public class SpotifyServiceTests : BaseServiceTests
    {
        [Fact]
        public async void Test1()
        {
            var spotifyService = _serviceProvider.GetService<ISpotifyService>();
            var a = spotifyService.AuthorizeUrl();
            string code = "";
            var b = spotifyService.RequestAccessRefreshToken(code);
        }
    }
}