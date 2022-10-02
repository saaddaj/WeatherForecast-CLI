using System.Globalization;
using System.Text.Json.Serialization;

namespace WeatherForecast.Cli.Models;
internal sealed record City
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }

    public City(string name, decimal latitude, decimal longitude)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString()
    {
        string latitude = Latitude.ToString(CultureInfo.InvariantCulture);
        string longitude = Longitude.ToString(CultureInfo.InvariantCulture);

        return $"{Name} [{latitude} - {longitude}]";
    }
}
