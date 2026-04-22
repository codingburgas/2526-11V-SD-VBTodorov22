using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.DTOs;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class DirectorService : IDirectorService
{
    private readonly IRepository<Director> _directorRepository;

    public DirectorService(IRepository<Director> directorRepository)
    {
        _directorRepository = directorRepository;
    }

    public async Task<IReadOnlyCollection<DirectorDto>> GetAllAsync()
    {
        return await _directorRepository.Query()
            .OrderBy(director => director.FullName)
            .Select(director => new DirectorDto
            {
                Id = director.Id,
                FullName = director.FullName,
                Biography = director.Biography,
                MovieCount = director.Movies.Count,
                CreatedAt = director.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<DirectorFormDto?> GetForEditAsync(int id)
    {
        return await _directorRepository.Query()
            .Where(director => director.Id == id)
            .Select(director => new DirectorFormDto
            {
                FullName = director.FullName,
                Biography = director.Biography
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(DirectorFormDto dto)
    {
        var director = new Director(dto.FullName, dto.Biography);
        await _directorRepository.AddAsync(director);
        await _directorRepository.SaveChangesAsync();

        return director.Id;
    }

    public async Task<bool> UpdateAsync(int id, DirectorFormDto dto)
    {
        var director = await _directorRepository.GetByIdAsync(id);
        if (director is null)
        {
            return false;
        }

        director.UpdateDetails(dto.FullName, dto.Biography);
        _directorRepository.Update(director);
        await _directorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var director = await _directorRepository.Query(trackChanges: true)
            .Include(item => item.Movies)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (director is null || director.Movies.Any())
        {
            return false;
        }

        _directorRepository.Delete(director);
        await _directorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IReadOnlyCollection<LookupDto>> GetOptionsAsync()
    {
        return await _directorRepository.Query()
            .OrderBy(director => director.FullName)
            .Select(director => new LookupDto
            {
                Id = director.Id,
                Name = director.FullName
            })
            .ToListAsync();
    }
}
