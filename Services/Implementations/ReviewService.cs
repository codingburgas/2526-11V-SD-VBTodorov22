using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Movie> _movieRepository;

    public ReviewService(
        IRepository<Review> reviewRepository,
        IRepository<Movie> movieRepository)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetByMovieIdAsync(int movieId)
    {
        return await _reviewRepository.Query()
            .Where(review => review.MovieId == movieId)
            .Include(review => review.User)
            .OrderByDescending(review => review.CreatedAt)
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserName = review.User!.UserName ?? "Unknown user",
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt
            })
            .ToListAsync();
    }

    public Task<bool> MovieExistsAsync(int movieId)
    {
        return _movieRepository.Query()
            .AnyAsync(movie => movie.Id == movieId);
    }

    public Task<bool> HasUserReviewedAsync(int movieId, string userId)
    {
        return _reviewRepository.Query()
            .AnyAsync(review => review.MovieId == movieId && review.UserId == userId);
    }

    public async Task<bool> AddAsync(ReviewFormDto dto, string userId)
    {
        var movieExists = await MovieExistsAsync(dto.MovieId);
        if (!movieExists)
        {
            return false;
        }

        var review = new Review(dto.MovieId, userId, dto.Comment, dto.Rating);
        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return true;
    }
}
