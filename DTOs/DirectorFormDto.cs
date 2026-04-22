using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.DTOs;

public class DirectorFormDto
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Director Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Biography { get; set; } = string.Empty;
}
