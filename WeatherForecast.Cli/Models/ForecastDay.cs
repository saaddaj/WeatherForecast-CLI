using System.Text.Json.Serialization;
using WeatherForecast.Cli.Models.JsonConverters;

namespace WeatherForecast.Cli.Models;

internal sealed record ForecastDay
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    [JsonPropertyName("day")]
    public Day Day { get; set; }

    public ForecastDay(DateOnly date, Day day)
    {
        Date = date;
        Day = day;
    }
}
