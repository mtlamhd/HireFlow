using HireFlow.Domain.Dtos.CategoryDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface ICategoryService
{
    Task<List<CategoryViewDto>> GetAllCategoriesAsync();
}