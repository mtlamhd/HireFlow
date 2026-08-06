using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.CityDto;
using HireFlow.Domain.Entities;
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
            throw new InvalidRequestException("Province ID cannot be empty.");
        }
        
        var provinceExists = await _unitOfWork.Provinces.AnyAsync(p => p.Id == provinceId);
        
        if (!provinceExists)
        {
            throw new ItemNotFoundException("Province", provinceId);
        }
        
        return await _unitOfWork.Cities.GetCitiesDtoByProvinceIdAsync(provinceId);
    }
    
    public async Task CreateCityAsync(string name, Guid provinceId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRequestException("City name cannot be empty.");

        if (provinceId == Guid.Empty)
            throw new InvalidRequestException("Province ID cannot be empty.");

        
        var provinceExists = await _unitOfWork.Provinces.AnyAsync(p => p.Id == provinceId);
        if (!provinceExists)
            throw new ItemNotFoundException("Province", provinceId);

        var trimmedName = name.Trim();

      
        var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Name == trimmedName && c.ProvinceId == provinceId);
        if (cityExists)
            throw new ConflictException($"City '{trimmedName}' already exists in this province.");

        var city = new City(trimmedName, provinceId);
        await _unitOfWork.Cities.AddAsync(city);
        await _unitOfWork.SaveChangesAsync();
    }
}