namespace MovieSeriesCatalog.Options;

public class CoverImageOptions
{
    public const string SectionName = "CoverImages";

    public string RelativeUploadPath { get; set; } = "uploads/covers";

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png"];

    public string[] AllowedContentTypes { get; set; } = ["image/jpeg", "image/png"];
}
