using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.DTOs;

public class ReviewFormDto
{
    [Required]
    public int MovieId { get; set; }

    public string MovieTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    [Display(Name = "Review Text")]
    public string Comment { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Rating { get; set; }
}
