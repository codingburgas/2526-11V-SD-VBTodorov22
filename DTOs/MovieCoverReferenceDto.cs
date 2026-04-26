namespace MovieSeriesCatalog.DTOs;

public class MovieCoverReferenceDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? CoverImageUrl { get; init; }
}
