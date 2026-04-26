using Microsoft.Extensions.Options;
using MovieSeriesCatalog.Options;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class CoverImageStorageService : ICoverImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly CoverImageOptions _options;

    public CoverImageStorageService(
        IWebHostEnvironment environment,
        IOptions<CoverImageOptions> options)
    {
        _environment = environment;
        _options = options.Value;
    }

    public async Task<string> SaveAsync(int movieId, IFormFile file, CancellationToken cancellationToken = default)
    {
        Validate(file);

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var relativeDirectory = NormalizeRelativeDirectory(_options.RelativeUploadPath);
        var absoluteDirectory = GetAbsoluteDirectory(relativeDirectory);

        Directory.CreateDirectory(absoluteDirectory);

        var fileName = $"film-{movieId}-{Guid.NewGuid():N}{fileExtension}";
        var absolutePath = Path.Combine(absoluteDirectory, fileName);

        try
        {
            await using var fileStream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fileStream, cancellationToken);
        }
        catch
        {
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            throw;
        }

        return $"/{relativeDirectory}/{fileName}";
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var normalizedDirectory = NormalizeRelativeDirectory(_options.RelativeUploadPath);
        var normalizedPath = relativePath.Trim().TrimStart('/', '\\').Replace('\\', '/');

        if (!normalizedPath.StartsWith($"{normalizedDirectory}/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var absoluteDirectory = Path.GetFullPath(GetAbsoluteDirectory(normalizedDirectory));
        var absolutePath = Path.GetFullPath(Path.Combine(
            GetWebRootPath(),
            normalizedPath.Replace('/', Path.DirectorySeparatorChar)));

        if (!absolutePath.StartsWith(absoluteDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }

    private void Validate(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new CoverImageValidationException("The uploaded file is empty.");
        }

        if (file.Length > _options.MaxFileSizeBytes)
        {
            throw new CoverImageValidationException("Cover images must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new CoverImageValidationException("Only JPG and PNG cover images are allowed.");
        }

        if (!_options.AllowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new CoverImageValidationException("The uploaded file content type is not supported.");
        }
    }

    private string GetAbsoluteDirectory(string relativeDirectory)
    {
        return Path.Combine(
            GetWebRootPath(),
            relativeDirectory.Replace('/', Path.DirectorySeparatorChar));
    }

    private string GetWebRootPath()
    {
        return string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;
    }

    private static string NormalizeRelativeDirectory(string relativeDirectory)
    {
        return relativeDirectory.Trim().Trim('/', '\\').Replace('\\', '/');
    }
}
