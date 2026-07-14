using HireFlow.Domain.Dtos.ProvinceDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IProvinceRepository : IGenericRepository<Province>
{
    Task<List<ProvinceViewDto>> GetAllProvincesDtoAsync();
}