namespace Bora.Wheather
{
    using System;
    using System.Collections.Generic;

    public class WeatherForecast
    {
        public Headline Headline { get; set; }
        public List<Forecast> DailyForecasts { get; set; }
    }
    public class Forecast
    {
        public DateTimeOffset Date { get; set; }
        public double? HoursOfSun { get; set; }
        public long? EpochDate { get; set; }
        public Sun Sun { get; set; }
        public Moon Moon { get; set; }
        public ForecastDetails Day { get; set; }
        public ForecastDetails Night { get; set; }
        public Temperature Temperature { get; set; }
        public Temperature RealFeelTemperature { get; set; }
        public Temperature RealFeelTemperatureShade { get; set; }
        public DegreeDaySummary DegreeDaySummary { get; set; }
        public List<AirAndPollen> AirAndPollen { get; set; }
        public List<string> Sources { get; set; }
        public string? MobileLink { get; set; }
        public string? Link { get; set; }
    }
    public class ForecastDetails
    {
        /// <summary>
        /// https://apidev.accuweather.com/developers/weatherIcons
        /// </summary>
        public int? Icon { get; set; }
        /// <summary>
        /// Phrase description of the icon.
        /// </summary>
        public string? IconPhrase { get; set; }
        /// <summary>
        /// Phrase description of the forecast. AccuWeather attempts to keep this phrase under 30 characters in length, but some languages/weather events may result in a phrase exceeding 30 characters
        /// </summary>
        public string? ShortPhrase { get; set; }
        /// <summary>
        /// Phrase description of the forecast. AccuWeather attempts to keep this phrase under 100 characters in length, but some languages/weather events may result in a phrase exceeding 100 characters.
        /// </summary>
        public string? LongPhrase { get; set; }
        /// <summary>
        /// indicates the presence of any type of precipitation for a given day. Displays true if precipitation is present
        /// </summary>
        public bool? HasPrecipitation { get; set; }
        public int PrecipitationType { get; set; }
        public int? ThunderstormProbability { get; set; }
        public int? PrecipitationProbability { get; set; }
        public double? HoursOfPrecipitation { get; set; }

        #region rain
        public double? HoursOfRain { get; set; }
        public int? RainProbability { get; set; }
        public ForecastUnit Rain { get; set; }
        #endregion

        #region snow
        public int? SnowProbability { get; set; }
        public double? HoursOfSnow { get; set; }
        public ForecastUnit Snow { get; set; }
        #endregion

        #region ice
        public int? IceProbability { get; set; }
        public ForecastUnit Ice { get; set; }
        public double? HoursOfIce { get; set; }
        #endregion        
        
        public ForecastUnit SolarIrradiance { get; set; }
        public ForecastUnit TotalLiquid { get; set; }
        public ForecastUnit Evapotranspiration { get; set; }
        
        public int? CloudCover { get; set; }
        public Wind Wind { get; set; }
        public Wind WindGust { get; set; }
        public RelativeHumidity RelativeHumidity { get; set; }
        public WetBulbTemperature WetBulbTemperature { get; set; }
        public WetBulbTemperature WetBulbGlobeTemperature { get; set; }
    }

    public class Headline
    {
        public string? Text { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public long? EffectiveEpochDate { get; set; }
        public int? Severity { get; set; }
        public string? Category { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public long? EndEpochDate { get; set; }
        public string? MobileLink { get; set; }
        public string? Link { get; set; }
    }

    public class Sun
    {
        public DateTimeOffset Rise { get; set; }
        public long? EpochRise { get; set; }
        public DateTimeOffset Set { get; set; }
        public long? EpochSet { get; set; }
    }

    public class Moon
    {
        public DateTime Rise { get; set; }
        public long? EpochRise { get; set; }
        public DateTime Set { get; set; }
        public long? EpochSet { get; set; }
        public string? Phase { get; set; }
        public int? Age { get; set; }
    }

    public class Temperature
    {
        public TemperatureUnit Minimum { get; set; }
        public TemperatureUnit Maximum { get; set; }
    }

    public class Wind
    {
        public ForecastUnit Speed { get; set; }
        public WindDirection Direction { get; set; }
    }

    

    public class WetBulbTemperature
    {
        public Temperature Minimum { get; set; }
        public Temperature Maximum { get; set; }
        public Temperature Average { get; set; }
    }
    

    public class DegreeDaySummary
    {
        public Temperature Heating { get; set; }
        public Temperature Cooling { get; set; }
    }

    public struct ForecastUnit
    {
        public double? Value { get; set; }
        public string? Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class TemperatureUnit
    {
        public double? Value { get; set; }
        public string? Unit { get; set; }
        public int? UnitType { get; set; }
        public string? Phrase { get; set; }
    }
    public class RelativeHumidity
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public int? Average { get; set; }
    }
    public class WindDirection
    {
        public int? Degrees { get; set; }
        public string? Localized { get; set; }
        public string? English { get; set; }
    }
    public class AirAndPollen
    {
        public string? Name { get; set; }
        public int? Value { get; set; }
        public string? Category { get; set; }
        public int? CategoryValue { get; set; }
        public string? Type { get; set; }
    }
}
