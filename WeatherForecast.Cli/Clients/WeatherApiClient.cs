using System.Globalization;
using System.Net.Http.Json;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Clients;
internal sealed class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;

    private readonly string _apiKey;

    public WeatherApiClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<Forecast?> GetNext2DaysForecastByCoordinatesAsync(decimal latitude, decimal longitude)
    {
        string endpoint = string.Format("/v1/forecast.json?key={0}&q={1},{2}&days=2",
            _apiKey,
            latitude.ToString(CultureInfo.InvariantCulture),
            longitude.ToString(CultureInfo.InvariantCulture));

        return await _httpClient.GetFromJsonAsync<Forecast>(endpoint).ConfigureAwait(false);
    }
}
