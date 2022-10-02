using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record ForecastWrapper
{
    [JsonPropertyName("forecast")]
    public Forecast Forecast { get; set; }

    public ForecastWrapper(Forecast forecast)
    {
        Forecast = forecast;
    }
}
