using MovieSeriesCatalog.DTOs;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyCollection<ReviewDto>> GetByMovieIdAsync(int movieId);

    Task<bool> MovieExistsAsync(int movieId);

    Task<bool> HasUserReviewedAsync(int movieId, string userId);

    Task<bool> AddAsync(ReviewFormDto dto, string userId);
}
