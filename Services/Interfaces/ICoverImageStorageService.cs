namespace MovieSeriesCatalog.Services.Interfaces;

public interface ICoverImageStorageService
{
    Task<string> SaveAsync(int movieId, IFormFile file, CancellationToken cancellationToken = default);

    void Delete(string? relativePath);
}
