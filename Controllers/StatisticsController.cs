using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _statisticsService.GetStatisticsAsync());
    }
}
