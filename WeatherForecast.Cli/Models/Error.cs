using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record Error
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public Error(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
