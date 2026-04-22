using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Director> _directorRepository;
    private readonly IRepository<Actor> _actorRepository;

    public MovieService(
        IRepository<Movie> movieRepository,
        IRepository<Director> directorRepository,
        IRepository<Actor> actorRepository)
    {
        _movieRepository = movieRepository;
        _directorRepository = directorRepository;
        _actorRepository = actorRepository;
    }

    public Task<IReadOnlyCollection<MovieListDto>> GetAllAsync()
    {
        return GetCatalogAsync();
    }

    public async Task<IReadOnlyCollection<MovieListDto>> GetCatalogAsync(
        GenreType? genre = null,
        int? year = null,
        CatalogType? catalogType = null)
    {
        var query = _movieRepository.Query()
            .Include(movie => movie.Director)
            .Include(movie => movie.Reviews)
            .AsQueryable();

        if (genre.HasValue)
        {
            query = query.Where(movie => movie.Genre == genre.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(movie => movie.ReleaseYear == year.Value);
        }

        if (catalogType.HasValue)
        {
            query = query.Where(movie => movie.CatalogType == catalogType.Value);
        }

        var movies = await query
            .OrderByDescending(movie => movie.ReleaseYear)
            .ThenBy(movie => movie.Title)
            .Select(movie => new
            {
                movie.Id,
                movie.CatalogType,
                movie.Title,
                movie.Description,
                movie.ReleaseYear,
                movie.Genre,
                DirectorName = movie.Director!.FullName,
                AverageRating = movie.Reviews.Select(review => (double?)review.Rating).Average() ?? 0,
                ReviewCount = movie.Reviews.Count
            })
            .ToListAsync();

        return movies
            .Select(movie => new MovieListDto
            {
                Id = movie.Id,
                CatalogType = movie.CatalogType,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                Genre = movie.Genre,
                DirectorName = movie.DirectorName,
                AverageRating = Math.Round(movie.AverageRating, 2),
                ReviewCount = movie.ReviewCount
            })
            .ToList();
    }

    public async Task<IReadOnlyCollection<int>> GetAvailableYearsAsync()
    {
        return await _movieRepository.Query()
            .Select(movie => movie.ReleaseYear)
            .Distinct()
            .OrderByDescending(year => year)
            .ToListAsync();
    }

    public async Task<MovieDetailsDto?> GetDetailsAsync(int id)
    {
        var movie = await _movieRepository.Query()
            .Include(item => item.Director)
            .Include(item => item.MovieActors)
                .ThenInclude(movieActor => movieActor.Actor)
            .Include(item => item.Reviews)
                .ThenInclude(review => review.User)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (movie is null)
        {
            return null;
        }

        return new MovieDetailsDto
        {
            Id = movie.Id,
            CatalogType = movie.CatalogType,
            Title = movie.Title,
            Description = movie.Description,
            ReleaseYear = movie.ReleaseYear,
            Genre = movie.Genre,
            DirectorName = movie.Director?.FullName ?? "Unknown",
            AverageRating = movie.Rating,
            ReviewCount = movie.Reviews.Count,
            Actors = movie.MovieActors
                .Where(movieActor => movieActor.Actor is not null)
                .Select(movieActor => movieActor.Actor!.FullName)
                .OrderBy(name => name)
                .ToList(),
            Reviews = movie.Reviews
                .OrderByDescending(review => review.CreatedAt)
                .Select(review => new ReviewDto
                {
                    Id = review.Id,
                    MovieId = review.MovieId,
                    UserName = review.User?.UserName ?? "Unknown user",
                    Comment = review.Comment,
                    Rating = review.Rating,
                    CreatedAt = review.CreatedAt
                })
                .ToList()
        };
    }

    public async Task<MovieFormDto?> GetForEditAsync(int id)
    {
        return await _movieRepository.Query()
            .Where(movie => movie.Id == id)
            .Select(movie => new MovieFormDto
            {
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                Genre = movie.Genre,
                CatalogType = movie.CatalogType,
                DirectorId = movie.DirectorId,
                ActorIds = movie.MovieActors.Select(movieActor => movieActor.ActorId).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(MovieFormDto dto)
    {
        await EnsureReferencesExistAsync(dto);

        var movie = new Movie(
            dto.CatalogType,
            dto.Title,
            dto.Description,
            dto.ReleaseYear,
            dto.Genre,
            dto.DirectorId);

        movie.SetActors(dto.ActorIds);

        await _movieRepository.AddAsync(movie);
        await _movieRepository.SaveChangesAsync();

        return movie.Id;
    }

    public async Task<bool> UpdateAsync(int id, MovieFormDto dto)
    {
        await EnsureReferencesExistAsync(dto);

        var movie = await _movieRepository.Query(trackChanges: true)
            .Include(item => item.MovieActors)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (movie is null)
        {
            return false;
        }

        movie.UpdateDetails(dto.CatalogType, dto.Title, dto.Description, dto.ReleaseYear, dto.Genre, dto.DirectorId);
        movie.SetActors(dto.ActorIds);

        _movieRepository.Update(movie);
        await _movieRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie is null)
        {
            return false;
        }

        _movieRepository.Delete(movie);
        await _movieRepository.SaveChangesAsync();

        return true;
    }

    private async Task EnsureReferencesExistAsync(MovieFormDto dto)
    {
        var directorExists = await _directorRepository.Query()
            .AnyAsync(director => director.Id == dto.DirectorId);

        if (!directorExists)
        {
            throw new InvalidOperationException("The selected director does not exist.");
        }

        var validActorCount = await _actorRepository.Query()
            .CountAsync(actor => dto.ActorIds.Contains(actor.Id));

        if (validActorCount != dto.ActorIds.Distinct().Count())
        {
            throw new InvalidOperationException("One or more selected actors do not exist.");
        }
    }
}
