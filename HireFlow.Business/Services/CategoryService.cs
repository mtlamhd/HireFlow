using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Entities;
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

    public async Task CreateCategoryAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRequestException("Category name cannot be empty.");

        var trimmedName = name.Trim();
    
       
        var exists = await _unitOfWork.Categories.AnyAsync(c => c.Name == trimmedName);
        if (exists)
            throw new ConflictException($"Category with name '{trimmedName}' already exists.");

        var category = new Category(trimmedName);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
    
}