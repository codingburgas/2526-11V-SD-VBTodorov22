using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.DTOs;

public class MovieCatalogDto
{
    public CatalogType? TypeFilter { get; init; }

    public GenreType? GenreFilter { get; init; }

    public int? YearFilter { get; init; }

    public IReadOnlyCollection<int> AvailableYears { get; init; } = Array.Empty<int>();

    public IReadOnlyCollection<MovieListDto> Movies { get; init; } = Array.Empty<MovieListDto>();
}
