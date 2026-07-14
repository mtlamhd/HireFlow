using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<List<CategoryViewDto>> GetAllCategoriesDtoAsync();
}