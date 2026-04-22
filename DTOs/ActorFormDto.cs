using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.DTOs;

public class ActorFormDto
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Actor Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Biography { get; set; } = string.Empty;
}
