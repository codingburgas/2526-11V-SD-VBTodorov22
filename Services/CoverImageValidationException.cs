namespace MovieSeriesCatalog.Services;

public class CoverImageValidationException : Exception
{
    public CoverImageValidationException(string message)
        : base(message)
    {
    }
}
