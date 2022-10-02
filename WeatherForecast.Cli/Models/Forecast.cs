using System.Text.Json.Serialization;
using WeatherForecast.Cli.Interfaces;

namespace WeatherForecast.Cli.Models;
internal sealed record Forecast : IQueryResult
{
    [JsonPropertyName("forecastday")]
    public ForecastDay[] ForecastDays { get; set; }

    public Forecast(ForecastDay[] forecastDays)
    {
        ForecastDays = forecastDays;
    }

    public string? GetConditionByIndex(int index)
    {
        if (index > ForecastDays.Length - 1)
            return null;

        ForecastDay forecastDay = ForecastDays[index];

        return forecastDay.Day.Condition.Text;
    }
}
