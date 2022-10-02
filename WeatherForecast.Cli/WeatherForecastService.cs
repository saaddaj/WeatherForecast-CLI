using WeatherForecast.Cli.Errors;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Models;

namespace WeatherForecast.Cli;
internal sealed class WeatherForecastService
{
    private readonly IMusementApiClient _musementApiClient;

    private readonly IWeatherApiClient _weatherApiClient;

    public WeatherForecastService(IMusementApiClient musementApiClient,
        IWeatherApiClient weatherApiClient)
    {
        _musementApiClient = musementApiClient;
        _weatherApiClient = weatherApiClient;
    }

    /// <summary>
    /// Gets the forecast for the next 2 days of cities where TUI Musement has activities to sell and prints it to the console
    /// </summary>
    public async Task ProcessCitiesAsync()
    {
        ICollection<City>? cities = await _musementApiClient.GetCitiesAsync().ConfigureAwait(false);

        if (cities == null)
        {
            Console.WriteLine("An error occured in the request to Musement API");
            return;
        }

        if (cities.Count == 0)
        {
            Console.WriteLine("No city was returned from Musement API");
            return;
        }

        await Parallel.ForEachAsync(cities, async (city, _) =>
        {
            IQueryResult? queryResult = await _weatherApiClient.GetNext2DaysForecastByCoordinatesAsync(
                city.Latitude,
                city.Longitude)
            .ConfigureAwait(false);

            if (queryResult == null)
            {
                Console.WriteLine($"No weather forecast found for city {city}");
                return;
            }

            switch (queryResult)
            {
                case Forecast forecast:
                    OutputCityForecast(city, forecast);
                    break;

                case ApiInternalError:
                    Console.WriteLine($"An error internal to weatherapi occured when requesting city {city}");
                    break;
            }

        });
    }

    private static void OutputCityForecast(City city, Forecast forecast)
    {
        string weatherToday = forecast.WeatherToday;
        string weatherTomorrow = forecast.WeatherTomorrow;

        Console.WriteLine($"Processed city {city.Name} | {weatherToday} - {weatherTomorrow}");
    }
}
