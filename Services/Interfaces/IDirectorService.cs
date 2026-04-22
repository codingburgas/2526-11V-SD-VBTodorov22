using MovieSeriesCatalog.DTOs;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IDirectorService : ICrudService<DirectorDto, DirectorFormDto>
{
    Task<IReadOnlyCollection<LookupDto>> GetOptionsAsync();
}
