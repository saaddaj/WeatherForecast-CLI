using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecast.Cli.Clients;
using WeatherForecast.Cli.Interfaces;
using WeatherForecast.Cli.Options;

namespace WeatherForecast.Cli;
internal static class RegisterDependencies
{
    public static void RegisterAll(this IServiceCollection services,
        IConfiguration configuration)
    {
        RegisterMusementApiClient(services, configuration);
        RegisterWeatherApiClient(services, configuration);

        services.AddSingleton<WeatherForecastService>();
    }

    private static void RegisterMusementApiClient(IServiceCollection services,
        IConfiguration configuration)
    {
        var musementApiOptions = configuration
            .GetSection(MusementApiOptions.SectionKey)
            .Get<MusementApiOptions>();

        services.AddHttpClient<IMusementApiClient, MusementApiClient>(
            c => c.BaseAddress = new Uri(musementApiOptions.BaseAddress))
            .SetHandlerLifetime(TimeSpan.Parse(musementApiOptions.HandlerLifeTime))
            .AddPolicyHandler(MusementApiClient.GetRetryPolicy());
    }

    private static void RegisterWeatherApiClient(IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection weatherApiSection = configuration.GetSection(
            WeatherApiOptions.SectionKey);

        services.Configure<WeatherApiOptions>(weatherApiSection);

        var weatherApiOptions = weatherApiSection.Get<WeatherApiOptions>();

        services.AddHttpClient<IWeatherApiClient, WeatherApiClient>(
            c => c.BaseAddress = new Uri(weatherApiOptions.BaseAddress))
            .SetHandlerLifetime(TimeSpan.Parse(weatherApiOptions.HandlerLifeTime))
            .AddPolicyHandler(WeatherApiClient.GetRetryPolicy());
    }
}
