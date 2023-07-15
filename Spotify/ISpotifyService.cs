using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;

namespace Spotify
{
    public interface ISpotifyService
    {
        string? AuthorizeUrl();
        Task<BearerAccessRefreshToken> RequestAccessRefreshToken(string code);
        Task<string> ClonePlaylist(string playlistId, string accessToken);
    }
}
