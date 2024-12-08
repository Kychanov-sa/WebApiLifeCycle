using Microsoft.AspNetCore.Mvc;
using WebApiLifeCycle.Filters;

namespace WebApiLifeCycle.Controllers
{
  [DeprecatedApi(From = "2024-10-11", Sunset = "2024-12-05")]
  [Obsolete]
  [ApiController]
  [Route("v1/weather")]
  public class DeprecatedWeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<DeprecatedWeatherForecastController> _logger;

    public DeprecatedWeatherForecastController(ILogger<DeprecatedWeatherForecastController> logger)
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
  }
}