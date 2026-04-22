using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class ActorsController : Controller
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService)
    {
        _actorService = actorService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _actorService.GetAllAsync());
    }

    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Create()
    {
        return View(new ActorFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Create(ActorFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _actorService.CreateAsync(dto);
        TempData["StatusMessage"] = "Actor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var actor = await _actorService.GetForEditAsync(id);
        if (actor is null)
        {
            return NotFound();
        }

        return View(actor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id, ActorFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var updated = await _actorService.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Actor updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = (await _actorService.GetAllAsync()).FirstOrDefault(item => item.Id == id);
        if (actor is null)
        {
            return NotFound();
        }

        return View(actor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _actorService.DeleteAsync(id);
        if (!deleted)
        {
            TempData["ErrorMessage"] = "The actor cannot be deleted because they are linked to one or more catalog items.";
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusMessage"] = "Actor deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
