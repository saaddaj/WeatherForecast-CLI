using System.Net;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Tests.Clients;
public class MusementApiClientTests
{
    private readonly HttpClientFactory _httpClientFactory = new("/api/v3/cities");

    [Fact]
    public async Task GetCitiesAsync_WhenOk_ReturnsExpectedCollection()
    {
        // Arrange
        _httpClientFactory.HttpStatusCode = HttpStatusCode.OK;

        List<City> expectedCities = new()
        {
            new("Milan", 45.464664m, 9.188540m),
            new("Rome", 41.902782m, 12.496366m),
            new("Rabat", 34.01325m, -6.83255m)
        };
        _httpClientFactory.Content = expectedCities;

        HttpClient httpClient = _httpClientFactory.CreateClient();

        MusementApiClient musementApiClient = new(httpClient);

        // Act
        ICollection<City> actualCities = await musementApiClient.GetCitiesAsync();

        // Assert
        actualCities.Should().BeEquivalentTo(expectedCities);
    }
}
