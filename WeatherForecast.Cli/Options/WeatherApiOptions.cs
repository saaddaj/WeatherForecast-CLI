namespace WeatherForecast.Cli.Options;
internal sealed class WeatherApiOptions
{
    public const string SectionKey = "WeatherApi";

    public string BaseAddress { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}
