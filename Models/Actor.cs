using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.Models;

public class Actor : BaseEntity
{
    private Actor()
    {
    }

    public Actor(string fullName, string biography)
    {
        UpdateDetails(fullName, biography);
    }

    [Required]
    [StringLength(100)]
    public string FullName { get; private set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Biography { get; private set; } = string.Empty;

    public ICollection<MovieActor> MovieActors { get; private set; } = new List<MovieActor>();

    public void UpdateDetails(string fullName, string biography)
    {
        FullName = fullName.Trim();
        Biography = biography.Trim();
    }
}
