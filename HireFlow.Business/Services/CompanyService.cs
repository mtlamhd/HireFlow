using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CompanyDetailsDto> GetMyCompanyAsync(Guid userId)
    {
        
        if (userId == Guid.Empty)
        {
            throw new Exception("User ID cannot be empty.");
        }
        
        var companyDto = await _unitOfWork.Companies.GetCompanyDetailsByOwnerIdAsync(userId);
        
        if (companyDto == null)
        {
            throw new Exception("Company not found for the current user.");
        }

        return companyDto;
    }

    public async Task UpdateMyCompanyAsync(Guid userId, UpdateCompanyDto dto)
    {
        
        if (userId == Guid.Empty)
        {
            throw new Exception("User ID cannot be empty.");
        }

        if (dto.CityId.HasValue && dto.CityId.Value != Guid.Empty)
        {
            var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId.Value);
            if (!cityExists)
            {
                throw new Exception("The specified city does not exist.");
            }
        }
        
        var uniqueCategoryIds = new List<Guid>();
        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            uniqueCategoryIds = dto.CategoryIds.Distinct().ToList();
            var validCategoriesCount = await _unitOfWork.Categories.CountAsync(c => uniqueCategoryIds.Contains(c.Id));

            if (validCategoriesCount != uniqueCategoryIds.Count)
            {
                throw new Exception("One or more category IDs are invalid.");
            }
        }
        
        var isUpdated = await _unitOfWork.Companies.UpdateCompanyAndCategoriesAsync(userId, dto, uniqueCategoryIds);
        
        if (!isUpdated)
        {
            throw new Exception("Company not found for the current user.");
        }

        
        await _unitOfWork.SaveChangesAsync();
    }
    
    
    public async Task SetMyCompanyLogoAsync(Guid userId, Guid attachmentId)
    {
        if (userId == Guid.Empty || attachmentId == Guid.Empty)
        {
            throw new Exception("Invalid parameters.");
        }

        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new Exception("Company not found for the current user.");
        }

        company.SetLogo(attachmentId, userId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveMyCompanyLogoAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new Exception("User ID cannot be empty.");
        }

        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new Exception("Company not found for the current user.");
        }

        company.RemoveLogo(userId);

        await _unitOfWork.SaveChangesAsync();
    }
}
    
