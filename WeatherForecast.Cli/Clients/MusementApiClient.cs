using System.Text.Json;
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
        using HttpResponseMessage responseMessage = await _httpClient.GetAsync("/api/v3/cities").ConfigureAwait(false);

        using Stream responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var cities = await JsonSerializer.DeserializeAsync<List<City>>(responseStream).ConfigureAwait(false);

        return cities ?? new List<City>();
    }
}
