﻿using WeatherForecast.Cli.Interfaces;

namespace WeatherForecast.Cli.Errors;
internal sealed record ApiInternalError() : IQueryResult;
