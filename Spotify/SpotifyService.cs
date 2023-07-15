using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;

namespace Spotify
{
    internal class SpotifyService : ISpotifyService
    {
        readonly IUserAccountsService _userAccountsService;
        readonly IPlaylistsApi _playlistsApi;
        readonly IUsersProfileApi _usersProfileApi;

        readonly string[] scopes = new[]{ "playlist-read-private", "playlist-modify-public", "playlist-read-collaborative", "playlist-modify-private", "user-modify-playback-state", "user-read-playback-state" };
        const string lucasfogliariniId = "12145833562";

        public SpotifyService(IUserAccountsService userAccountsService,
                             IPlaylistsApi playlistsApi,
                             IUsersProfileApi usersProfileApi)
        {
            _userAccountsService = userAccountsService;
            _playlistsApi = playlistsApi;
            _usersProfileApi = usersProfileApi;
        }

        public string? AuthorizeUrl()
        {
            string state = Guid.NewGuid().ToString("N");
            var url = _userAccountsService.AuthorizeUrl(state, scopes);
            return url;
        }
        public async Task<BearerAccessRefreshToken> RequestAccessRefreshToken(string code)
        {
            var bearerAccessRefreshToken = await _userAccountsService.RequestAccessRefreshToken(code);
            return bearerAccessRefreshToken;
        }

        public async Task<string> ClonePlaylist(string playlistId, string accessToken)
        {
            var playlist = await _playlistsApi.GetPlaylist(playlistId, accessToken);
            var playlistPaged = await _playlistsApi.GetTracks(playlistId, accessToken);
            var trackUris = playlistPaged.Items.OrderByDescending(e => e.Track.Popularity).Select(e=>e.Track.Uri);
            return await CreatePlaylist(playlist.Name, trackUris, accessToken);
        }

        private async Task<string> CreatePlaylist(string name, IEnumerable<string> trackUris, string accessToken)
        {
            var user = await _usersProfileApi.GetCurrentUsersProfile(accessToken);
            var playlist = await _playlistsApi.CreatePlaylist(user.Id, new PlaylistDetails
            {
                Name = name,
                Public = false,
            }, accessToken);

            for (int i = 0; i < trackUris.Count(); i+= 100)
            {
                var trackUris100 = trackUris.Skip(i).Take(100).ToArray();
                await _playlistsApi.AddItemsToPlaylist(playlist.Id, trackUris100, accessToken: accessToken);
            }

            return playlist.Href;
        }
    }
}
