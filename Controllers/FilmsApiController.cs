using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Services;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Controllers;

[ApiController]
public class FilmsApiController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ICoverImageStorageService _coverImageStorageService;
    private readonly ILogger<FilmsApiController> _logger;

    public FilmsApiController(
        IMovieService movieService,
        ICoverImageStorageService coverImageStorageService,
        ILogger<FilmsApiController> logger)
    {
        _movieService = movieService;
        _coverImageStorageService = coverImageStorageService;
        _logger = logger;
    }

    [HttpGet("/films/{id:int}")]
    [ProducesResponseType(typeof(FilmApiDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FilmApiDto>> GetFilm(int id)
    {
        var film = await _movieService.GetDetailsAsync(id);
        if (film is null)
        {
            return NotFound(new { message = "Film not found." });
        }

        return Ok(new FilmApiDto
        {
            Id = film.Id,
            Title = film.Title,
            Description = film.Description,
            CoverUrl = film.CoverImageUrl,
            ReleaseYear = film.ReleaseYear,
            Genre = film.Genre,
            CatalogType = film.CatalogType,
            DirectorName = film.DirectorName,
            AverageRating = film.AverageRating,
            ReviewCount = film.ReviewCount,
            Actors = film.Actors
        });
    }

    [HttpPost("/admin/films/{id:int}/cover")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(CoverUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CoverUploadResponseDto>> UploadCover(
        int id,
        [FromForm] CoverUploadRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            return BadRequest(new { message = "A cover image file is required." });
        }

        var film = await _movieService.GetCoverReferenceAsync(id);
        if (film is null)
        {
            return NotFound(new { message = "Film not found." });
        }

        string? newCoverImageUrl = null;

        try
        {
            newCoverImageUrl = await _coverImageStorageService.SaveAsync(id, request.File, cancellationToken);

            var updated = await _movieService.UpdateCoverImageAsync(id, newCoverImageUrl);
            if (!updated)
            {
                _coverImageStorageService.Delete(newCoverImageUrl);
                return NotFound(new { message = "Film not found." });
            }

            _coverImageStorageService.Delete(film.CoverImageUrl);

            return Ok(new CoverUploadResponseDto
            {
                FilmId = film.Id,
                Title = film.Title,
                CoverImageUrl = newCoverImageUrl,
                Message = "Cover image uploaded successfully."
            });
        }
        catch (CoverImageValidationException exception)
        {
            if (!string.IsNullOrWhiteSpace(newCoverImageUrl))
            {
                _coverImageStorageService.Delete(newCoverImageUrl);
            }

            return BadRequest(new { message = exception.Message });
        }
        catch (IOException exception)
        {
            if (!string.IsNullOrWhiteSpace(newCoverImageUrl))
            {
                _coverImageStorageService.Delete(newCoverImageUrl);
            }

            _logger.LogError(exception, "Cover upload failed for film {FilmId}.", id);
            return Problem(
                detail: "The cover image could not be saved to disk.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
