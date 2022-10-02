using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record Day
{
    [JsonPropertyName("condition")]
    public Condition Condition { get; set; }

    public Day(Condition condition)
    {
        Condition = condition;
    }
}
