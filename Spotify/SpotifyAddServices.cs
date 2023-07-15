using Microsoft.Extensions.DependencyInjection;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;

namespace Spotify
{
    public static class SpotifyAddServices
    {
        public static void AddSpotifyService(this IServiceCollection services)
        {
            services.AddTransient<ISpotifyService, SpotifyService>();
            services.AddTransient<IPlaylistsApi, PlaylistsApi>();
            services.AddTransient<IUsersProfileApi, UsersProfileApi>();

            //how to configure: https://github.com/Ringobot/SpotifyApi.NetCore#user-authorization
            //app: https://developer.spotify.com/dashboard/applications/8d853260f0b7480889c1d6e43bc83676
            services.AddTransient<IUserAccountsService, UserAccountsService>();

            services.AddHttpClient();
        }
    }
}
