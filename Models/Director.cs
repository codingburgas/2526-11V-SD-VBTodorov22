using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.Models;

public class Director : BaseEntity
{
    private Director()
    {
    }

    public Director(string fullName, string biography)
    {
        UpdateDetails(fullName, biography);
    }

    [Required]
    [StringLength(100)]
    public string FullName { get; private set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Biography { get; private set; } = string.Empty;

    public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

    public void UpdateDetails(string fullName, string biography)
    {
        FullName = fullName.Trim();
        Biography = biography.Trim();
    }
}
