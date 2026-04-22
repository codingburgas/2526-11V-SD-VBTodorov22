using Microsoft.AspNetCore.Identity;

namespace MovieSeriesCatalog.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
}
