using HireFlow.Domain.Dtos.CityDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface ICityService
{
    Task<List<CityViewDto>> GetCitiesByProvinceIdAsync(Guid provinceId);
}