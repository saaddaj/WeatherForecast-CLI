using WeatherForecast.Cli.Interfaces;

namespace WeatherForecast.Cli.Models;
internal sealed record Forecast(string WeatherToday, string WeatherTomorrow) : IQueryResult;
