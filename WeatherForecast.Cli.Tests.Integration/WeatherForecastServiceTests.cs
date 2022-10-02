using Microsoft.Extensions.Configuration;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Tests.Integration;
public class WeatherForecastServiceTests
{
    [Fact]
    public async Task ProcessCitiesAsyncTest()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var musementApiOptions = configuration
            .GetSection(MusementApiOptions.SectionKey)
            .Get<MusementApiOptions>();

        HttpClient musementHttpClient = new()
        {
            BaseAddress = new Uri(musementApiOptions.BaseAddress)
        };
        IMusementApiClient musementApiClient = new MusementApiClient(musementHttpClient);

        var weatherApiOptions = configuration
            .GetSection(WeatherApiOptions.SectionKey)
            .Get<WeatherApiOptions>();

        HttpClient weatherHttpClient = new()
        {
            BaseAddress = new Uri(weatherApiOptions.BaseAddress)
        };
        IWeatherApiClient weatherApiClient = new WeatherApiClient(
            weatherHttpClient, weatherApiOptions.ApiKey);

        WeatherForecastService weatherForecastService = new(musementApiClient, weatherApiClient);

        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Act
        await weatherForecastService.ProcessCitiesAsync();

        // Assert
        stringWriter
            .ToString()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should()
            .ContainMatch("Processed city *");
    }
}
