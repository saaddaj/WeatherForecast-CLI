using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using WeatherForecast.Cli.Errors;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Clients;
internal sealed class WeatherApiClient : IWeatherApiClient
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));
    }

    private readonly HttpClient _httpClient;

    private readonly string _apiKey;

    public WeatherApiClient(HttpClient httpClient,
        IOptions<WeatherApiOptions> weatherApiOptions)
    {
        _httpClient = httpClient;
        _apiKey = weatherApiOptions.Value.ApiKey;
    }

    public async Task<IQueryResult?> GetNext2DaysForecastByCoordinatesAsync(decimal latitude, decimal longitude)
    {
        string endpoint = string.Format("/v1/forecast.json?key={0}&q={1},{2}&days=2",
            _apiKey,
            latitude.ToString(CultureInfo.InvariantCulture),
            longitude.ToString(CultureInfo.InvariantCulture));

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(
            endpoint).ConfigureAwait(false);

        using Stream responseStream = await responseMessage.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var forecastWrapper = await JsonSerializer.DeserializeAsync<ForecastWrapper>(
                responseStream).ConfigureAwait(false);

            return forecastWrapper?.Forecast;
        }

        var errorWrapper = await JsonSerializer.DeserializeAsync<ErrorWrapper>(
            responseStream).ConfigureAwait(false);

        Error? error = errorWrapper?.Error;

        return error?.Code switch
        {
            #pragma warning disable format
            9999                                            => new ApiInternalError(),
            1002 or 1003 or 1005 or 2006 or 2007 or 2008    => new ApiConfigurationError(error?.Message),
            #pragma warning restore format

            _ => null
        };
    }
}
