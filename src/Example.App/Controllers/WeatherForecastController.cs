using kr.bbon.AspNetCore;
using kr.bbon.Core;
using Microsoft.AspNetCore.Mvc;

namespace Example.App.Controllers;

[ApiController]
[Route(DefaultValues.RouteTemplate)]
[Area(DefaultValues.AreaName)]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {

        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        _logger.LogInformation("Result is {@result}", result);

        return result;
    }

    [HttpPost("400")]
    public IActionResult BadRequest()
    {
        throw new ApiException(StatusCodes.Status400BadRequest, "CASE #400: HTTP 400 Response");
    }

    [HttpDelete("500")]
    public IActionResult InternalServerError()
    {
        throw new ApiException(StatusCodes.Status500InternalServerError, "CASE #500: HTTP 500 Response");
    }
}
