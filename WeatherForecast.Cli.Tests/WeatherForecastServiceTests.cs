using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Tests;
public class WeatherForecastServiceTests
{
    [Fact]
    public async Task ProcessCitiesAsync_WhenOk_OutputsWeatherForecastsToTheConsole()
    {
        // Arrange
        List<City> cities = new()
        {
            new("Milan", 45.464664m, 9.188540m),
            new("Rome", 41.902782m, 12.496366m)
        };
        Mock<IMusementApiClient> musementApiClientMock = new();
        musementApiClientMock.Setup(m => m.GetCitiesAsync()).ReturnsAsync(cities);

        Dictionary<(decimal Latitude, decimal Longitude), Forecast> forecasts = new()
        {
            // Milan
            [(45.464664m, 9.188540m)] = new("Heavy rain", "Partly cloudy"),
            // Rome
            [(41.902782m, 12.496366m)] = new("Sunny", "Sunny")
        };
        Mock<IWeatherApiClient> weatherApiClientMock = new();
        weatherApiClientMock.Setup(
                w => w.GetNext2DaysForecastByCoordinatesAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()))
            .ReturnsAsync(
                (decimal lat, decimal lon) => forecasts.GetValueOrDefault((lat, lon)));

        WeatherForecastService weatherForecastService = new(
            musementApiClientMock.Object,
            weatherApiClientMock.Object);

        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Act
        await weatherForecastService.ProcessCitiesAsync();

        // Assert
        stringWriter
            .ToString()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should()
            .BeEquivalentTo(new[]
            {
                "Processed city Milan | Heavy rain - Partly cloudy",
                "Processed city Rome | Sunny - Sunny"
            });
    }
}
