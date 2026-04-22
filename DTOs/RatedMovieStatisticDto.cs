namespace MovieSeriesCatalog.DTOs;

public class RatedMovieStatisticDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }
}
