using System.Globalization;

namespace WeatherForecast.Cli.Models;
internal sealed record City(string Name, decimal Latitude, decimal Longitude)
{
    public override string ToString()
    {
        return $"{Name} " +
            $"[{Latitude.ToString(CultureInfo.InvariantCulture)} - " +
            $"{Longitude.ToString(CultureInfo.InvariantCulture)}]";
    }
}
