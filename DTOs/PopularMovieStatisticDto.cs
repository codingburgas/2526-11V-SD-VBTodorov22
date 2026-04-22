namespace MovieSeriesCatalog.DTOs;

public class PopularMovieStatisticDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int ReviewCount { get; init; }

    public double AverageRating { get; init; }
}
