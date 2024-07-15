using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Bora.Wheather
{
    public static class AccuWheatherHttpClientExtension
    {
        public static IServiceCollection AddAccuWheatherHttpClient(this IServiceCollection serviceCollection, string accuWheatherApiDomain, string accuWheatherApiKey)
        {
            serviceCollection.AddHttpClient<IWheather, AccuWheatherHttpClient>((provider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(accuWheatherApiDomain);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", accuWheatherApiKey);
            });
            return serviceCollection;
        }
    }
}
