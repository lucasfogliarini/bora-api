using System.Text.Json;

namespace Bora.Wheather
{
    public class AccuWheatherHttpClient(HttpClient httpClient) : IWheather
    {
        public async Task<WeatherForecast> GetForecastAsync(string days = "1day")
        {
            var apiKey = httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            ArgumentException.ThrowIfNullOrEmpty(httpClient.DefaultRequestHeaders.Authorization?.Parameter);
            const string locationKeyPOA = "45561";

            var requestUri = $"forecasts/v1/daily/{days}/{locationKeyPOA}?apikey={apiKey}&language=pt-BR&details=true";
            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            var eventsString = await response.Content.ReadAsStringAsync();
            var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(eventsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return weatherForecast;
        }
    }
}
