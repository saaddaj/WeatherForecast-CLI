using System.Net.Http.Json;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Clients;
internal sealed class MusementApiClient : IMusementApiClient
{
    private readonly HttpClient _httpClient;

    public MusementApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ICollection<City>> GetCitiesAsync()
    {
        List<City>? cities = await _httpClient.GetFromJsonAsync<List<City>>(
            "/api/v3/cities").ConfigureAwait(false);

        return cities ?? new List<City>();
    }
}
