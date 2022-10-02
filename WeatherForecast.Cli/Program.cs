using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherForecast.Cli;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .Build();

using IHost host = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.RegisterAll(configuration))
    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;

var weatherForecastService = serviceProvider.GetRequiredService<WeatherForecastService>();
await weatherForecastService.ProcessCitiesAsync();
