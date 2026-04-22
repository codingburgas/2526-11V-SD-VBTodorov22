using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class StatisticsService : IStatisticsService
{
    private readonly IRepository<Movie> _movieRepository;

    public StatisticsService(IRepository<Movie> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var topRated = await _movieRepository.Query()
            .Where(movie => movie.CatalogType == CatalogType.Movie)
            .Select(movie => new
            {
                movie.Id,
                movie.Title,
                AverageRating = movie.Reviews.Select(review => (double?)review.Rating).Average() ?? 0,
                ReviewCount = movie.Reviews.Count
            })
            .OrderByDescending(movie => movie.AverageRating)
            .ThenByDescending(movie => movie.ReviewCount)
            .ThenBy(movie => movie.Title)
            .Take(5)
            .ToListAsync();

        var moviesPerGenre = await _movieRepository.Query()
            .Where(movie => movie.CatalogType == CatalogType.Movie)
            .GroupBy(movie => movie.Genre)
            .Select(group => new GenreCountDto
            {
                Genre = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Genre)
            .ToListAsync();

        var mostPopular = await _movieRepository.Query()
            .Where(movie => movie.CatalogType == CatalogType.Movie)
            .Select(movie => new
            {
                movie.Id,
                movie.Title,
                ReviewCount = movie.Reviews.Count,
                AverageRating = movie.Reviews.Select(review => (double?)review.Rating).Average() ?? 0
            })
            .OrderByDescending(movie => movie.ReviewCount)
            .ThenByDescending(movie => movie.AverageRating)
            .ThenBy(movie => movie.Title)
            .Take(5)
            .ToListAsync();

        return new StatisticsDto
        {
            TopRatedMovies = topRated
                .Select(movie => new RatedMovieStatisticDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    AverageRating = Math.Round(movie.AverageRating, 2),
                    ReviewCount = movie.ReviewCount
                })
                .ToList(),
            MoviesPerGenre = moviesPerGenre,
            MostPopularMovies = mostPopular
                .Select(movie => new PopularMovieStatisticDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ReviewCount = movie.ReviewCount,
                    AverageRating = Math.Round(movie.AverageRating, 2)
                })
                .ToList()
        };
    }
}
