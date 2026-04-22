namespace MovieSeriesCatalog.Services.Interfaces;

public interface ICrudService<TListDto, TFormDto>
{
    Task<IReadOnlyCollection<TListDto>> GetAllAsync();

    Task<TFormDto?> GetForEditAsync(int id);

    Task<int> CreateAsync(TFormDto dto);

    Task<bool> UpdateAsync(int id, TFormDto dto);

    Task<bool> DeleteAsync(int id);
}
