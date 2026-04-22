using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IActorService _actorService;
    private readonly IDirectorService _directorService;

    public MoviesController(
        IMovieService movieService,
        IActorService actorService,
        IDirectorService directorService)
    {
        _movieService = movieService;
        _actorService = actorService;
        _directorService = directorService;
    }

    public async Task<IActionResult> Index(CatalogType? catalogType, GenreType? genre, int? year)
    {
        var movies = await _movieService.GetCatalogAsync(genre, year, catalogType);
        var years = await _movieService.GetAvailableYearsAsync();

        var model = new MovieCatalogDto
        {
            TypeFilter = catalogType,
            GenreFilter = genre,
            YearFilter = year,
            AvailableYears = years,
            Movies = movies
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetDetailsAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Create()
    {
        await PopulateSelectionsAsync();

        return View(new MovieFormDto
        {
            ReleaseYear = DateTime.UtcNow.Year,
            CatalogType = CatalogType.Movie
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Create(MovieFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(dto);
            return View(dto);
        }

        try
        {
            await _movieService.CreateAsync(dto);
            TempData["StatusMessage"] = "Catalog item created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateSelectionsAsync(dto);
            return View(dto);
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _movieService.GetForEditAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        await PopulateSelectionsAsync(movie);
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id, MovieFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(dto);
            return View(dto);
        }

        try
        {
            var updated = await _movieService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }

            TempData["StatusMessage"] = "Catalog item updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateSelectionsAsync(dto);
            return View(dto);
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _movieService.GetDetailsAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Catalog item deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSelectionsAsync(MovieFormDto? dto = null)
    {
        ViewBag.Directors = new SelectList(
            await _directorService.GetOptionsAsync(),
            "Id",
            "Name",
            dto?.DirectorId);

        ViewBag.Actors = new MultiSelectList(
            await _actorService.GetOptionsAsync(),
            "Id",
            "Name",
            dto?.ActorIds);
    }
}
