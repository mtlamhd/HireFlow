using HireFlow.Domain.Dtos.ProvinceDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IProvinceService
{
    Task<List<ProvinceViewDto>> GetAllProvincesAsync();
}