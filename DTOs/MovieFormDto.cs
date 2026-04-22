using System.ComponentModel.DataAnnotations;
using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.DTOs;

public class MovieFormDto
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(1888, 2100)]
    [Display(Name = "Release Year")]
    public int ReleaseYear { get; set; }

    [Required]
    [Display(Name = "Genre")]
    public GenreType Genre { get; set; }

    [Required]
    [Display(Name = "Catalog Type")]
    public CatalogType CatalogType { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select a director.")]
    [Display(Name = "Director")]
    public int DirectorId { get; set; }

    [MinLength(1, ErrorMessage = "Select at least one actor.")]
    [Display(Name = "Actors")]
    public List<int> ActorIds { get; set; } = new();
}
