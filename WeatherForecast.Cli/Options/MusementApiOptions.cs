namespace WeatherForecast.Cli.Options;
internal sealed class MusementApiOptions
{
    public const string SectionKey = "MusementApi";

    public string BaseAddress { get; set; } = string.Empty;

    public string HandlerLifeTime { get; set; } = string.Empty;
}
