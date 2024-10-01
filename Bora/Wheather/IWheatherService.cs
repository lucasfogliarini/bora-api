namespace Bora.Wheather
{
    public interface IWheatherService
    {
        Task<WeatherForecast> GetForecastAsync(string days = "5day");
    }
}
