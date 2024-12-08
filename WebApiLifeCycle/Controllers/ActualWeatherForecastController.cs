using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using WebApiLifeCycle.Filters;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApiLifeCycle.Controllers
{
  [ApiController]
  [Route("v2/weather")]
  public class ActualWeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<ActualWeatherForecastController> _logger;

    public ActualWeatherForecastController(ILogger<ActualWeatherForecastController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
    }

    [ExperimentalApi]
    [Experimental(diagnosticId:"E1233")]
    [HttpGet("extended")]
    public IEnumerable<WeatherForecastEx> GetExtended()
    {
      return Get().Select(f => new WeatherForecastEx()
      {
        Date = f.Date,
        TemperatureC = f.TemperatureC,
        Summary = f.Summary,
        MoonPhase = Moon.Calculate(f.Date.ToDateTime(TimeOnly.MinValue)).ToString(),
      }).ToArray();
    }
  }
}