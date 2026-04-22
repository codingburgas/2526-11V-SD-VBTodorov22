using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class ActorService : IActorService
{
    private readonly IRepository<Actor> _actorRepository;

    public ActorService(IRepository<Actor> actorRepository)
    {
        _actorRepository = actorRepository;
    }

    public async Task<IReadOnlyCollection<ActorDto>> GetAllAsync()
    {
        return await _actorRepository.Query()
            .OrderBy(actor => actor.FullName)
            .Select(actor => new ActorDto
            {
                Id = actor.Id,
                FullName = actor.FullName,
                Biography = actor.Biography,
                MovieCount = actor.MovieActors.Count,
                CreatedAt = actor.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ActorFormDto?> GetForEditAsync(int id)
    {
        return await _actorRepository.Query()
            .Where(actor => actor.Id == id)
            .Select(actor => new ActorFormDto
            {
                FullName = actor.FullName,
                Biography = actor.Biography
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(ActorFormDto dto)
    {
        var actor = new Actor(dto.FullName, dto.Biography);
        await _actorRepository.AddAsync(actor);
        await _actorRepository.SaveChangesAsync();

        return actor.Id;
    }

    public async Task<bool> UpdateAsync(int id, ActorFormDto dto)
    {
        var actor = await _actorRepository.GetByIdAsync(id);
        if (actor is null)
        {
            return false;
        }

        actor.UpdateDetails(dto.FullName, dto.Biography);
        _actorRepository.Update(actor);
        await _actorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var actor = await _actorRepository.Query(trackChanges: true)
            .Include(item => item.MovieActors)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (actor is null || actor.MovieActors.Any())
        {
            return false;
        }

        _actorRepository.Delete(actor);
        await _actorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IReadOnlyCollection<LookupDto>> GetOptionsAsync()
    {
        return await _actorRepository.Query()
            .OrderBy(actor => actor.FullName)
            .Select(actor => new LookupDto
            {
                Id = actor.Id,
                Name = actor.FullName
            })
            .ToListAsync();
    }
}
