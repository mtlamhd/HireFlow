using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.ProvinceDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class ProvinceService : IProvinceService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProvinceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProvinceViewDto>> GetAllProvincesAsync()
    {
        return await _unitOfWork.Provinces.GetAllProvincesDtoAsync();
    }
    public async Task CreateProvinceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRequestException("Province name cannot be empty.");

        var trimmedName = name.Trim();

        var exists = await _unitOfWork.Provinces.AnyAsync(p => p.Name == trimmedName);
        if (exists)
            throw new ConflictException($"Province with name '{trimmedName}' already exists.");

        var province = new Province(trimmedName);
        await _unitOfWork.Provinces.AddAsync(province);
        await _unitOfWork.SaveChangesAsync();
    }
}