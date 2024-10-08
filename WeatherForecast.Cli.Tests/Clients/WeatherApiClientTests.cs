﻿using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Errors;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Tests.Clients;
public class WeatherApiClientTests
{
    private const string _apiKey = "testApiKey";

    private const decimal _latitude = 45.464664m;

    private const decimal _longitude = 9.188540m;

    private readonly HttpClientFactory _httpClientFactory;

    private readonly IOptions<WeatherApiOptions> _weatherApiOptions;

    public WeatherApiClientTests()
    {
        const string apiKey = "testApiKey";

        string request = string.Format("/v1/forecast.json?key={0}&q={1},{2}&days=2",
            apiKey,
            _latitude.ToString(CultureInfo.InvariantCulture),
            _longitude.ToString(CultureInfo.InvariantCulture));

        _httpClientFactory = new HttpClientFactory(request);

        _weatherApiOptions = Microsoft.Extensions.Options.Options.Create(
            new WeatherApiOptions() { ApiKey = apiKey });
    }

    [Fact]
    public async Task GetForecastByCoordinatesAsync_WhenOk_ReturnsExpectedForecast()
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = HttpStatusCode.OK;

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        Forecast expectedForecast = new(new ForecastDay[]
        {
            new(today, new Day(new Condition("Sunny"))),
            new(today.AddDays(1), new Day(new Condition("Partly cloudy"))),
        });
        _httpClientFactory.Content = new ForecastWrapper(expectedForecast);

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        var actualForecast = queryResult.Should().BeOfType<Forecast>().Subject;
        actualForecast.Should().BeEquivalentTo(expectedForecast);
    }

    [Fact]
    public async Task GetForecastByCoordinatesAsync_WhenErrorCode1006_ReturnsNull()
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = HttpStatusCode.BadRequest;

        const int code = 1006;
        const string message = "No location found matching parameter 'q'";
        _httpClientFactory.Content = new Error(code, message);

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().BeNull();
    }

    [Fact]
    public async Task GetForecastByCoordinatesAsync_WhenErrorCode9999_ReturnsApiInternalError()
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = HttpStatusCode.BadRequest;

        const int code = 9999;
        const string message = "Internal application error";
        _httpClientFactory.Content = new ErrorWrapper(new Error(code, message));

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        queryResult.Should().BeOfType<ApiInternalError>();
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, 1002, "API key not provided")]
    [InlineData(HttpStatusCode.BadRequest, 1003, "Parameter 'q' not provided")]
    [InlineData(HttpStatusCode.BadRequest, 1005, "API request url is invalid")]
    [InlineData(HttpStatusCode.Unauthorized, 2006, "API key provided is invalid")]
    [InlineData(HttpStatusCode.Forbidden, 2007, "API key has exceeded calls per month quota")]
    [InlineData(HttpStatusCode.Forbidden, 2008, "API key has been disabled")]
    public async Task GetForecastByCoordinatesAsync_WhenSpecificErrorCodes_ReturnsApiConfigurationError(HttpStatusCode httpStatusCode, int errorCode, string errorMessage)
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = httpStatusCode;

        _httpClientFactory.Content = new ErrorWrapper(new Error(errorCode, errorMessage));

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _weatherApiOptions);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        var apiConfigurationError = queryResult.Should().BeOfType<ApiConfigurationError>().Subject;
        apiConfigurationError.Message.Should().Be(errorMessage);
    }
}
