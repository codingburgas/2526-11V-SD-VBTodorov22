namespace MovieSeriesCatalog.DTOs;

public class ReviewDto
{
    public int Id { get; init; }

    public int MovieId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Comment { get; init; } = string.Empty;

    public int Rating { get; init; }

    public DateTime CreatedAt { get; init; }
}
