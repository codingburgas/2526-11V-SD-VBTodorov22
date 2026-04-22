using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.Models;

public enum CatalogType
{
    [Display(Name = "Movie")]
    Movie = 1,

    [Display(Name = "Series")]
    Series = 2
}
