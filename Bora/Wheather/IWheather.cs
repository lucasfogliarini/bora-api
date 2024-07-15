namespace Bora.Wheather
{
    public interface IWheather
    {
        Task<WeatherForecast> GetForecastAsync(string days = "5day");
    }
}
