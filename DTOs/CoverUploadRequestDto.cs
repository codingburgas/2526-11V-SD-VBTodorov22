using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.DTOs;

public class CoverUploadRequestDto
{
    [Required]
    public IFormFile? File { get; init; }
}
