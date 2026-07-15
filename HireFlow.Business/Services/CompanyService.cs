using HireFlow.Business.Exceptionss;
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
            throw new InvalidRequestException("User ID cannot be empty.");
        }
        
        var companyDto = await _unitOfWork.Companies.GetCompanyDetailsByOwnerIdAsync(userId);
        
        if (companyDto == null)
        {
            throw new ItemNotFoundException($"Company for user with id '{userId}' was not found.");
        }

        return companyDto;
    }

    public async Task UpdateMyCompanyAsync(Guid userId, UpdateCompanyDto dto)
    {
        
        if (userId == Guid.Empty)
        {
            throw new InvalidRequestException("User ID cannot be empty.");
        }

        if (dto.CityId.HasValue && dto.CityId.Value != Guid.Empty)
        {
            var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId.Value);
            if (!cityExists)
            {
                throw new ItemNotFoundException("City", dto.CityId.Value);
            }
        }
        
        var uniqueCategoryIds = new List<Guid>();
        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            uniqueCategoryIds = dto.CategoryIds.Distinct().ToList();
            var validCategoriesCount = await _unitOfWork.Categories.CountAsync(c => uniqueCategoryIds.Contains(c.Id));

            if (validCategoriesCount != uniqueCategoryIds.Count)
            {
                throw new ItemNotFoundException("One or more specified categories were not found.");
            }
        }
        
        var isUpdated = await _unitOfWork.Companies.UpdateCompanyAndCategoriesAsync(userId, dto, uniqueCategoryIds);
        
        if (!isUpdated)
        {
            throw new ItemNotFoundException($"Company for user with id '{userId}' was not found.");
        }

        
        await _unitOfWork.SaveChangesAsync();
    }
    
    
    public async Task SetMyCompanyLogoAsync(Guid userId, Guid attachmentId)
    {
        if (userId == Guid.Empty || attachmentId == Guid.Empty)
        {
            throw new InvalidRequestException("User ID and Attachment ID cannot be empty.");
        }

        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new ItemNotFoundException($"Company for user with id '{userId}' was not found.");
        }

        company.SetLogo(attachmentId, userId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveMyCompanyLogoAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidRequestException("User ID cannot be empty.");
        }

        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new ItemNotFoundException($"Company for user with id '{userId}' was not found.");
        }

        company.RemoveLogo(userId);

        await _unitOfWork.SaveChangesAsync();
    }
}
    
