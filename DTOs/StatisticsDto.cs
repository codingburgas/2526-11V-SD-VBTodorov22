namespace MovieSeriesCatalog.DTOs;

public class StatisticsDto
{
    public IReadOnlyCollection<RatedMovieStatisticDto> TopRatedMovies { get; init; } = Array.Empty<RatedMovieStatisticDto>();

    public IReadOnlyCollection<GenreCountDto> MoviesPerGenre { get; init; } = Array.Empty<GenreCountDto>();

    public IReadOnlyCollection<PopularMovieStatisticDto> MostPopularMovies { get; init; } = Array.Empty<PopularMovieStatisticDto>();
}
