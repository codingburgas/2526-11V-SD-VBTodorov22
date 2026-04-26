using System.Net.Http.Headers;

namespace FilmStreamingPlatform.Examples;

public sealed class AdminPanelUploadExample
{
    private readonly HttpClient _httpClient;

    public AdminPanelUploadExample(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> UploadCoverAsync(int filmId, string filePath, CancellationToken cancellationToken = default)
    {
        await using var fileStream = File.OpenRead(filePath);

        using var multipartContent = new MultipartFormDataContent();
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(filePath));
        multipartContent.Add(fileContent, "file", Path.GetFileName(filePath));

        using var response = await _httpClient.PostAsync($"/admin/films/{filmId}/cover", multipartContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private static string GetContentType(string filePath)
    {
        return Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => throw new InvalidOperationException("Only JPG and PNG files are supported.")
        };
    }
}
