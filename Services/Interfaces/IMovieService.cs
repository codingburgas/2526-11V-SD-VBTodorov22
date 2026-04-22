using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IMovieService : ICrudService<MovieListDto, MovieFormDto>
{
    Task<IReadOnlyCollection<MovieListDto>> GetCatalogAsync(
        GenreType? genre = null,
        int? year = null,
        CatalogType? catalogType = null);

    Task<IReadOnlyCollection<int>> GetAvailableYearsAsync();

    Task<MovieDetailsDto?> GetDetailsAsync(int id);
}
