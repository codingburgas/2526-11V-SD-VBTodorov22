using System.ComponentModel.DataAnnotations;

namespace MovieSeriesCatalog.Models;

public enum GenreType
{
    [Display(Name = "Action")]
    Action = 1,

    [Display(Name = "Adventure")]
    Adventure = 2,

    [Display(Name = "Comedy")]
    Comedy = 3,

    [Display(Name = "Crime")]
    Crime = 4,

    [Display(Name = "Drama")]
    Drama = 5,

    [Display(Name = "Fantasy")]
    Fantasy = 6,

    [Display(Name = "Sci-Fi")]
    SciFi = 7,

    [Display(Name = "Thriller")]
    Thriller = 8
}
