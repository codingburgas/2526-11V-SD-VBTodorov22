using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieSeriesCatalog.Models;

public class Movie : BaseEntity
{
    private Movie()
    {
    }

    public Movie(
        CatalogType catalogType,
        string title,
        string description,
        int releaseYear,
        GenreType genre,
        int directorId)
    {
        UpdateDetails(catalogType, title, description, releaseYear, genre, directorId);
    }

    [Required]
    [StringLength(150)]
    public string Title { get; private set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; private set; } = string.Empty;

    [Range(1888, 2100)]
    public int ReleaseYear { get; private set; }

    [Required]
    public GenreType Genre { get; private set; }

    [Required]
    public CatalogType CatalogType { get; private set; }

    public int DirectorId { get; private set; }

    public Director? Director { get; private set; }

    public ICollection<MovieActor> MovieActors { get; private set; } = new List<MovieActor>();

    public ICollection<Review> Reviews { get; private set; } = new List<Review>();

    [NotMapped]
    public double Rating => Reviews.Count == 0
        ? 0
        : Math.Round(Reviews.Average(review => review.Rating), 2);

    public void UpdateDetails(
        CatalogType catalogType,
        string title,
        string description,
        int releaseYear,
        GenreType genre,
        int directorId)
    {
        CatalogType = catalogType;
        Title = title.Trim();
        Description = description.Trim();
        ReleaseYear = releaseYear;
        Genre = genre;
        DirectorId = directorId;
    }

    public void SetActors(IEnumerable<int> actorIds)
    {
        var distinctActorIds = actorIds
            .Distinct()
            .ToList();

        MovieActors.Clear();

        foreach (var actorId in distinctActorIds)
        {
            MovieActors.Add(new MovieActor(actorId));
        }
    }
}
