using System.Globalization;
using System.Net;
using System.Text.Json;
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

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            using Stream responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return await JsonSerializer.DeserializeAsync<Forecast>(responseStream).ConfigureAwait(false);
        }

        return null;
    }
}
