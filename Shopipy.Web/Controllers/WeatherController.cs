using Microsoft.AspNetCore.Mvc;

namespace Shopipy.Web.Controllers;

public class WeatherController : Controller
{
    private readonly WeatherApiClient _weatherApi;

    public WeatherController(WeatherApiClient weatherApi)
    {
        _weatherApi = weatherApi;
    }

    public async Task<IActionResult> Index()
    {
        var forecasts = await _weatherApi.GetWeatherAsync();
        return View(forecasts);
    }
}