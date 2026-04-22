using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.Models;

public class Review : BaseEntity
{
    private Review()
    {
    }

    public Review(int movieId, string userId, string comment, int rating)
    {
        MovieId = movieId;
        UserId = userId;
        Update(comment, rating);
    }

    [Required]
    [StringLength(1000)]
    public string Comment { get; private set; } = string.Empty;

    [Range(1, 10)]
    public int Rating { get; private set; }

    public int MovieId { get; private set; }

    public Movie? Movie { get; private set; }

    [Required]
    public string UserId { get; private set; } = string.Empty;

    public ApplicationUser? User { get; private set; }

    public void Update(string comment, int rating)
    {
        Comment = comment.Trim();
        Rating = rating;
    }
}
