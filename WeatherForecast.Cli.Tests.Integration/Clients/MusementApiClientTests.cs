using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli.Tests.Integration.Clients;
public class MusementApiClientTests
{
    [Fact]
    public async Task GetCitiesAsyncTest()
    {
        // Arrange
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://sandbox.musement.com")
        };

        MusementApiClient musementApiClient = new(httpClient);

        // Act
        ICollection<City>? cities = await musementApiClient.GetCitiesAsync();

        // Assert
        cities.Should().NotBeNull();
        cities.Should().NotBeEmpty();
    }
}
