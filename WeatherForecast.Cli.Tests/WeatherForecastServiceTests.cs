using WeatherForecast.Cli.Errors;
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

    [Fact]
    public async Task ProcessCitiesAsync_WhenCityCollectionNull_OutputsSpecificMessageToTheConsole()
    {
        // Arrange
        List<City>? cities = null;
        Mock<IMusementApiClient> musementApiClientMock = new();
        musementApiClientMock.Setup(m => m.GetCitiesAsync()).ReturnsAsync(cities);

        Mock<IWeatherApiClient> weatherApiClientMock = new();

        WeatherForecastService weatherForecastService = new(
            musementApiClientMock.Object,
            weatherApiClientMock.Object);

        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Act
        await weatherForecastService.ProcessCitiesAsync();

        // Assert
        stringWriter.ToString().Trim().Should().Be("An error occured in the request to Musement API");
    }

    [Fact]
    public async Task ProcessCitiesAsync_WhenCityCollectionEmpty_OutputsSpecificMessageToTheConsole()
    {
        // Arrange
        List<City> cities = new();
        Mock<IMusementApiClient> musementApiClientMock = new();
        musementApiClientMock.Setup(m => m.GetCitiesAsync()).ReturnsAsync(cities);

        Mock<IWeatherApiClient> weatherApiClientMock = new();

        WeatherForecastService weatherForecastService = new(
            musementApiClientMock.Object,
            weatherApiClientMock.Object);

        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Act
        await weatherForecastService.ProcessCitiesAsync();

        // Assert
        stringWriter.ToString().Trim().Should().Be("No city was returned from Musement API");
    }

    [Fact]
    public async Task ProcessCitiesAsync_WhenCityHasNoForecast_OutputsSpecificMessageToTheConsole()
    {
        // Arrange
        City city = new("Milan", 45.464664m, 9.188540m);
        Mock<IMusementApiClient> musementApiClientMock = new();
        musementApiClientMock
            .Setup(m => m.GetCitiesAsync())
            .ReturnsAsync(new List<City> { city });

        Mock<IWeatherApiClient> weatherApiClientMock = new();

        WeatherForecastService weatherForecastService = new(
            musementApiClientMock.Object,
            weatherApiClientMock.Object);

        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Act
        await weatherForecastService.ProcessCitiesAsync();

        // Assert
        stringWriter.ToString().Trim().Should().Be($"No weather forecast found for city {city}");
    }

    [Fact]
    public async Task ProcessCitiesAsync_WhenApiInternalError_OutputsSpecificMessageToTheConsole()
    {
        // Arrange
        City city = new("Milan", 45.464664m, 9.188540m);
        Mock<IMusementApiClient> musementApiClientMock = new();
        musementApiClientMock
            .Setup(m => m.GetCitiesAsync())
            .ReturnsAsync(new List<City> { city });

        Mock<IWeatherApiClient> weatherApiClientMock = new();
        weatherApiClientMock.Setup(
                w => w.GetNext2DaysForecastByCoordinatesAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()))
            .ReturnsAsync(new ApiInternalError());

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
            .Trim()
            .Should()
            .Be($"An error internal to weatherapi occured when requesting city {city}");
    }
}
