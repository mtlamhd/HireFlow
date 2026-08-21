using HireFlow.Domain.Dtos.ProvinceDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Repositories;

public class ProvinceRepository : GenericRepository<Province>,IProvinceRepository
{
    public ProvinceRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<ProvinceViewDto>> GetAllProvincesDtoAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(p => new ProvinceViewDto()
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
    }
}