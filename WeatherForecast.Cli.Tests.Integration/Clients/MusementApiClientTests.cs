using Microsoft.Extensions.Configuration;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Models;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli.Tests.Integration.Clients;
public class MusementApiClientTests
{
    [Fact]
    public async Task GetCitiesAsyncTest()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var musementApiOptions = configuration
            .GetSection(MusementApiOptions.SectionKey)
            .Get<MusementApiOptions>();

        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(musementApiOptions.BaseAddress)
        };

        MusementApiClient musementApiClient = new(httpClient);

        // Act
        ICollection<City>? cities = await musementApiClient.GetCitiesAsync();

        // Assert
        cities.Should().NotBeNull();
        cities.Should().NotBeEmpty();
        City city = cities!.First();
        string.IsNullOrWhiteSpace(city.Name).Should().BeFalse();
        city.Latitude.Should().BeGreaterThan(0);
        city.Longitude.Should().BeGreaterThan(0);
    }
}
