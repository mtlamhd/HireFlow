using HireFlow.Domain.Dtos.CityDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class CityRepository : GenericRepository<City>,ICityRepository
{
    public CityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<CityViewDto>> GetCitiesDtoByProvinceIdAsync(Guid provinceId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ProvinceId == provinceId)
            .Select(c => new CityViewDto
            {
                Id = c.Id,
                Name = c.Name,
                ProvinceId = c.ProvinceId
            })
            .ToListAsync();
    }
}