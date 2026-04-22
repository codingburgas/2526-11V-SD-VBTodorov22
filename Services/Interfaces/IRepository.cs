using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    IQueryable<TEntity> Query(bool trackChanges = false);

    Task<TEntity?> GetByIdAsync(int id);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task<int> SaveChangesAsync();
}
