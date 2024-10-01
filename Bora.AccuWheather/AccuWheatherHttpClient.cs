using Bora.Wheather;
using System.Net.Http.Json;
using System.Text.Json;

namespace Bora.AccuWheather
{
    public class AccuWheatherHttpClient(HttpClient httpClient) : IWheatherService
    {
        public async Task<WeatherForecast> GetForecastAsync(string days = "5day")
        {
            var apiKey = httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            ArgumentException.ThrowIfNullOrEmpty(httpClient.DefaultRequestHeaders.Authorization?.Parameter);
            const string locationKeyPOA = "45561";

            var requestUri = $"forecasts/v1/daily/{days}/{locationKeyPOA}?apikey={apiKey}&language=pt-BR&details=true";
            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            var weatherForecast = await response.Content.ReadFromJsonAsync<WeatherForecast>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return weatherForecast;
        }
    }
}
