using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoryViewDto>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetAllCategoriesDtoAsync();
    }
    
}