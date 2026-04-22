using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IMovieService _movieService;

    public ReviewsController(IReviewService reviewService, IMovieService movieService)
    {
        _reviewService = reviewService;
        _movieService = movieService;
    }

    public async Task<IActionResult> Index(int movieId)
    {
        var movie = await _movieService.GetDetailsAsync(movieId);
        if (movie is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var canManageOwnReview = !string.IsNullOrWhiteSpace(userId)
            && (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.User));
        var hasExistingUserReview = canManageOwnReview
            && await _reviewService.HasUserReviewedAsync(movieId, userId!);

        var model = new MovieReviewsPageDto
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            AverageRating = movie.AverageRating,
            CanAddReview = canManageOwnReview && !hasExistingUserReview,
            HasExistingUserReview = hasExistingUserReview,
            Reviews = await _reviewService.GetByMovieIdAsync(movieId)
        };

        return View(model);
    }

    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.User}")]
    public async Task<IActionResult> Create(int movieId)
    {
        var movie = await _movieService.GetDetailsAsync(movieId);
        if (movie is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId)
            && await _reviewService.HasUserReviewedAsync(movieId, userId))
        {
            TempData["ErrorMessage"] = "You have already submitted a review for this catalog item.";
            return RedirectToAction(nameof(Index), new { movieId });
        }

        return View(BuildReviewForm(movie));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.User}")]
    public async Task<IActionResult> Create(ReviewFormDto dto)
    {
        var movie = await _movieService.GetDetailsAsync(dto.MovieId);
        if (movie is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(BuildReviewForm(movie, dto));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (await _reviewService.HasUserReviewedAsync(dto.MovieId, userId))
        {
            ModelState.AddModelError(string.Empty, "You have already submitted a review for this catalog item.");
            return View(BuildReviewForm(movie, dto));
        }

        var added = await _reviewService.AddAsync(dto, userId);
        if (!added)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Review added successfully.";
        return RedirectToAction(nameof(Index), new { movieId = dto.MovieId });
    }

    private static ReviewFormDto BuildReviewForm(MovieDetailsDto movie, ReviewFormDto? draft = null)
    {
        return new ReviewFormDto
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            Rating = draft?.Rating ?? 8,
            Comment = draft?.Comment ?? string.Empty
        };
    }
}
