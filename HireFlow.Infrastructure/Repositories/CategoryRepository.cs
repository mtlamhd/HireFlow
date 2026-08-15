using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class CategoryRepository : GenericRepository<Category>,ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<List<CategoryViewDto>> GetAllCategoriesDtoAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(c => new CategoryViewDto()
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }
}