using MovieSeriesCatalog.DTOs;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IActorService : ICrudService<ActorDto, ActorFormDto>
{
    Task<IReadOnlyCollection<LookupDto>> GetOptionsAsync();
}
