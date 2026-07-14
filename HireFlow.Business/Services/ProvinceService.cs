using HireFlow.Domain.Dtos.ProvinceDto;
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
}