using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Errors;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Tests.Integration.Clients;
public class WeatherApiClientTests
{
    private readonly HttpClient _httpClient;

    private readonly string _apiKey;

    private const decimal _latitude = 45.464664m;

    private const decimal _longitude = 9.188540m;

    public WeatherApiClientTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var weatherApiOptions = configuration
            .GetSection(WeatherApiOptions.SectionKey)
            .Get<WeatherApiOptions>();

        _httpClient = new()
        {
            BaseAddress = new Uri(weatherApiOptions.BaseAddress)
        };

        _apiKey = weatherApiOptions.ApiKey;
    }

    [Fact]
    public async Task GetNext2DaysForecastByCoordinatesAsync_WithApiKey_ReturnsForecast()
    {
        // Arrange
        IOptions<WeatherApiOptions> weatherApiOptions = Microsoft.Extensions.Options.Options.Create(
            new WeatherApiOptions() { ApiKey = _apiKey });

        WeatherApiClient weatherApiClient = new(_httpClient, weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        var forecast = queryResult.Should().BeOfType<Forecast>().Subject;
        string.IsNullOrWhiteSpace(forecast.GetConditionByIndex(0)).Should().BeFalse();
        string.IsNullOrWhiteSpace(forecast.GetConditionByIndex(1)).Should().BeFalse();
    }

    [Fact]
    public async Task GetNext2DaysForecastByCoordinatesAsync_WithInvalidApiKey_ReturnsForecast()
    {
        // Arrange
        IOptions<WeatherApiOptions> weatherApiOptions = Microsoft.Extensions.Options.Options.Create(
            new WeatherApiOptions() { ApiKey = "invalidApiKey" });

        WeatherApiClient weatherApiClient = new(_httpClient, weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        queryResult.Should().BeOfType<ApiConfigurationError>();
    }
}
