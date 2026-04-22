using MovieSeriesCatalog.DTOs;

namespace MovieSeriesCatalog.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsDto> GetStatisticsAsync();
}
