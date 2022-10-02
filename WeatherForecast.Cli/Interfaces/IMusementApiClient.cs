using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Interfaces;
internal interface IMusementApiClient
{
    /// <summary>
    /// Gets the collection of cities
    /// </summary>
    public Task<ICollection<City>?> GetCitiesAsync();
}
