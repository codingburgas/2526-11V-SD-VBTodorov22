namespace MovieSeriesCatalog.DTOs;

public class CoverUploadResponseDto
{
    public int FilmId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string CoverImageUrl { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
