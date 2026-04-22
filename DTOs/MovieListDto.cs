using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.DTOs;

public class MovieListDto
{
    public int Id { get; init; }

    public CatalogType CatalogType { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int ReleaseYear { get; init; }

    public GenreType Genre { get; init; }

    public string DirectorName { get; init; } = string.Empty;

    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }
}
