# C# .NET 6 Weather Forecast CLI
## A cross-plateform console application that fetches the weather forecast for the next 2 days for each of the cities where TUI Musement has activities to sell

This project is my attempt at solving TUI Musement technical challenge of creating a weather forecast CLI application.

The application does the following:
- request all of the cities by calling Musement's API endpoint `GET /api/v3/cities`
- request the corresponding weather forecast of each city for the next 2 days by calling one of weatherapi's forecast API endpoint
- print it all out to the console using the specific template `Processed city [city name] | [weather today] - [wheather tomorrow]`

## How to use the application
Using docker you can run the application by doing the following :
1. Clone this repository
2. Modify the appsettings.json file, at the root of the `WeatherForecast.Cli` project, to provide a valid weatherapi key under the `WeatherApi:ApiKey` setting
3. Open a terminal inside of the `WeatherForecast.Cli` folder
3. Build a docker image by using the following command: `docker build -t weather-forecast-cli-image -f Dockerfile .`
4. Start a single run by using the following command: `docker run -it --rm weather-forecast-cli-image`

If you don't already have a valid weatherapi key, you can get one by signing up for free here: https://www.weatherapi.com/signup.aspx

## How the application is organized
The solution is composed of three projects:
- The `WeatherForecast.Cli` project which is the console application, containing all of the logic
- The `WeatherForecast.Cli.Tests` project which is the unit test project, containing the tests for individualized blocks of the application
- The `WeatherForecast.Cli.Tests.Integration` project which is the integration test project, containing the tests for blocks of the application in real conditions

You are welcome to clone this repository or submit pull requests to this one.

## If you wish to contribute
I would ask you to take into consideration the following items:
- The solution comes with an .editorconfig file containing the standard rules used by the IDE for coding
- I use SonarLint extension to help prevent code flaws and maintain clean code at all time
- I use FormatOnSave extension to apply a set of configured formatting rules each time I save my files, here is the configuration used on this repository:
>   - Enable UnifyEndOfFile: True
>   - Enable FormatDocument: True
>   - Enable UnifyLineBreak: True
>   - Line break style: Windows
>   - Enable RemoveAndSort: True
>   - Enable SmatRemoveAndSort: True
>   - Enable TabToSpace: True
>   - Enable ForceUtf8WithBom: True

You are welcome to use any IDE that is compatible with these recommendations, I personnaly use Visual Studio Community 2022
