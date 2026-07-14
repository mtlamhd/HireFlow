using HireFlow.Domain.Dtos.CityDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class CityService : ICityService
{
    private readonly IUnitOfWork _unitOfWork;

    public CityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CityViewDto>> GetCitiesByProvinceIdAsync(Guid provinceId)
    {
        if (provinceId == Guid.Empty)
        {
            throw new Exception("Province ID cannot be empty.");
        }
        
        var provinceExists = await _unitOfWork.Provinces.AnyAsync(p => p.Id == provinceId);
        
        if (!provinceExists)
        {
            throw new Exception("The specified province does not exist.");
        }
        
        return await _unitOfWork.Cities.GetCitiesDtoByProvinceIdAsync(provinceId);
    }
}