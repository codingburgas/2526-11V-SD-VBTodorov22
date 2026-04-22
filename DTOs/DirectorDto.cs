namespace MovieSeriesCatalog.DTOs;

public class DirectorDto
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Biography { get; init; } = string.Empty;

    public int MovieCount { get; init; }

    public DateTime CreatedAt { get; init; }
}
