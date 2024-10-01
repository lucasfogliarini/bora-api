using Bora.AccuWheather;
using Bora.Wheather;
using System.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AccuWheatherHttpClientExtension
    {
        const string accuweather_domain = "https://dataservice.accuweather.com";
        public static IServiceCollection AddAccuWheatherService(this IServiceCollection serviceCollection, string accuWheatherApiKey)
        {
            serviceCollection.AddHttpClient<IWheatherService, AccuWheatherHttpClient>((provider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(accuweather_domain);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", accuWheatherApiKey);
            });
            return serviceCollection;
        }
    }
}
