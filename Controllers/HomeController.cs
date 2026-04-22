using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class HomeController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IStatisticsService _statisticsService;

    public HomeController(IMovieService movieService, IStatisticsService statisticsService)
    {
        _movieService = movieService;
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredTitles = (await _movieService.GetCatalogAsync())
            .Take(3)
            .ToList();

        var statistics = await _statisticsService.GetStatisticsAsync();

        var model = new HomePageDto
        {
            FeaturedTitles = featuredTitles,
            TopRatedMovies = statistics.TopRatedMovies.Take(3).ToList()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
