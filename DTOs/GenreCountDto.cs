using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.DTOs;

public class GenreCountDto
{
    public GenreType Genre { get; init; }

    public int Count { get; init; }
}
