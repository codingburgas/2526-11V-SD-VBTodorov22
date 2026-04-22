namespace MovieSeriesCatalog.DTOs;

public class MovieReviewsPageDto
{
    public int MovieId { get; init; }

    public string MovieTitle { get; init; } = string.Empty;

    public double AverageRating { get; init; }

    public bool CanAddReview { get; init; }

    public bool HasExistingUserReview { get; init; }

    public IReadOnlyCollection<ReviewDto> Reviews { get; init; } = Array.Empty<ReviewDto>();
}
