using System.Net;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Clients;
internal sealed class MusementApiClient : IMusementApiClient
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromMinutes(Math.Pow(2, retryAttempt)));
    }

    private readonly HttpClient _httpClient;

    public MusementApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ICollection<City>?> GetCitiesAsync()
    {
        using HttpResponseMessage responseMessage = await _httpClient.GetAsync("/api/v3/cities").ConfigureAwait(false);

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            return new List<City>();

        using Stream responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<List<City>>(responseStream).ConfigureAwait(false);
    }
}
