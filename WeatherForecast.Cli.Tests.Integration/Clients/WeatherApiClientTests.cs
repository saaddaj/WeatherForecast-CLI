using Microsoft.Extensions.Configuration;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Tests.Integration.Clients;
public class WeatherApiClientTests
{
    [Fact]
    public async Task GetNext2DaysForecastByCoordinatesAsync_WithApiKey_ReturnsForecast()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var weatherApiOptions = configuration
            .GetSection(WeatherApiOptions.SectionKey)
            .Get<WeatherApiOptions>();

        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(weatherApiOptions.BaseAddress)
        };

        WeatherApiClient weatherApiClient = new(httpClient, weatherApiOptions.ApiKey);

        // Milan
        const decimal latitude = 45.464664m;
        const decimal longitude = 9.188540m;

        // Act
        IQueryResult? queryResult = await weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
            latitude,
            longitude);

        // Assert
        queryResult.Should().NotBeNull();
        var forecast = queryResult.Should().BeOfType<Forecast>().Subject;
        string.IsNullOrWhiteSpace(forecast.GetConditionByIndex(0)).Should().BeFalse();
        string.IsNullOrWhiteSpace(forecast.GetConditionByIndex(1)).Should().BeFalse();
    }
}
