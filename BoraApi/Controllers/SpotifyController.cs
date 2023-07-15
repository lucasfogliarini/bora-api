using Microsoft.AspNetCore.Mvc;
using Spotify;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpotifyController : BaseController
    {
        private readonly ISpotifyService _spotifyService;

        public SpotifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var url = _spotifyService.AuthorizeUrl();
            return Ok(url);
        }

        [HttpGet("token")]
        public async Task<IActionResult> Token(string code, string? state, string? error = null)
        {
            var bearerAccessRefreshToken = await _spotifyService.RequestAccessRefreshToken(code);
            return Ok(bearerAccessRefreshToken.AccessToken);
        }

        [HttpPost("playlists/{playlistId}/clone")]
        public async Task<IActionResult> Playlists(string playlistId, string accessToken)
        {
            var playlistHref = await _spotifyService.ClonePlaylist(playlistId, accessToken);
            return Ok(playlistHref);
        }
    }
}
