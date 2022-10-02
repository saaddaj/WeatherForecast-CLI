using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record Condition
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    public Condition(string text)
    {
        Text = text;
    }
}
