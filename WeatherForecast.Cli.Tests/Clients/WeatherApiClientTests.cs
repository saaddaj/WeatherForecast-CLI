﻿using System.Globalization;
using System.Net;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Errors;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Tests.Clients;
public class WeatherApiClientTests
{
    private const string _apiKey = "testApiKey";

    private const decimal _latitude = 45.464664m;

    private const decimal _longitude = 9.188540m;

    private readonly HttpClientFactory _httpClientFactory;

    public WeatherApiClientTests()
    {
        string request = string.Format("/v1/forecast.json?key={0}&q={1},{2}&days=2",
            _apiKey,
            _latitude.ToString(CultureInfo.InvariantCulture),
            _longitude.ToString(CultureInfo.InvariantCulture));

        _httpClientFactory = new HttpClientFactory(request);
    }

    [Fact]
    public async Task GetForecastByCoordinatesAsync_WhenOk_ReturnsExpectedForecast()
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = HttpStatusCode.OK;

        Forecast expectedForecast = new("Sunny", "Partly cloudy");
        _httpClientFactory.Content = expectedForecast;

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _apiKey);

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

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _apiKey);

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
        _httpClientFactory.Content = new Error(code, message);

        WeatherApiClient weatherApiClient = new(_httpClientFactory.CreateClient(), _apiKey);

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            _latitude,
            _longitude);

        // Assert
        queryResult.Should().NotBeNull();
        queryResult.Should().BeOfType<ApiInternalError>();
    }
}
