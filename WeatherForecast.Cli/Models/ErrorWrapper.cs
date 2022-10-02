using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record ErrorWrapper
{
    [JsonPropertyName("error")]
    public Error Error { get; set; }

    public ErrorWrapper(Error error)
    {
        Error = error;
    }
}
