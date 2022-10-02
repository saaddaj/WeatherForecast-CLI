namespace WeatherForecast.Cli.Interfaces;
internal interface IWeatherApiClient
{
    /// <summary>
    /// Gets the weather forecast for the next 2 days using the given coordinates
    /// </summary>
    public Task<IQueryResult?> GetNext2DaysForecastByCoordinatesAsync(decimal latitude, decimal longitude);
}
