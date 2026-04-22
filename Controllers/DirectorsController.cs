using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class DirectorsController : Controller
{
    private readonly IDirectorService _directorService;

    public DirectorsController(IDirectorService directorService)
    {
        _directorService = directorService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _directorService.GetAllAsync());
    }

    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Create()
    {
        return View(new DirectorFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Create(DirectorFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _directorService.CreateAsync(dto);
        TempData["StatusMessage"] = "Director created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var director = await _directorService.GetForEditAsync(id);
        if (director is null)
        {
            return NotFound();
        }

        return View(director);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id, DirectorFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var updated = await _directorService.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Director updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var director = (await _directorService.GetAllAsync()).FirstOrDefault(item => item.Id == id);
        if (director is null)
        {
            return NotFound();
        }

        return View(director);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _directorService.DeleteAsync(id);
        if (!deleted)
        {
            TempData["ErrorMessage"] = "The director cannot be deleted because they are linked to one or more catalog items.";
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusMessage"] = "Director deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
