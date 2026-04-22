namespace MovieSeriesCatalog.DTOs;

public class HomePageDto
{
    public IReadOnlyCollection<MovieListDto> FeaturedTitles { get; init; } = Array.Empty<MovieListDto>();

    public IReadOnlyCollection<RatedMovieStatisticDto> TopRatedMovies { get; init; } = Array.Empty<RatedMovieStatisticDto>();
}
