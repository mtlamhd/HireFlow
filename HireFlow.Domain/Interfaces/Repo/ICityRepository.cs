using HireFlow.Domain.Dtos.CityDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface ICityRepository : IGenericRepository<City>
{
    Task<List<CityViewDto>> GetCitiesDtoByProvinceIdAsync(Guid provinceId);
}