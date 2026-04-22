using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog.Services.Implementations;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(bool trackChanges = false)
    {
        return trackChanges
            ? _dbSet
            : _dbSet.AsNoTracking();
    }

    public Task<TEntity?> GetByIdAsync(int id)
    {
        return _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
