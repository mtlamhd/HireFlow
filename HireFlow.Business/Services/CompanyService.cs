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
        var companyDto = await _unitOfWork.Companies.GetCompanyDetailsByOwnerIdAsync(userId);

        if (companyDto == null)
        {
            throw new Exception("Company not found for the current user.");
        }

        return companyDto;
    }

    public async Task UpdateMyCompanyAsync(Guid userId, UpdateCompanyDto dto)
    {
        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new Exception("Company not found for the current user.");
        }
        
        company.UpdateInfo(
            dto.Name,
            dto.Description,
            dto.Website,
            dto.Email,
            dto.PhoneNumber,
            dto.Address,
            userId 
        );

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SetMyCompanyLogoAsync(Guid userId, Guid attachmentId)
    {
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
        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId, tracking: true);

        if (company == null)
        {
            throw new Exception("Company not found for the current user.");
        }

        company.RemoveLogo(userId);

        await _unitOfWork.SaveChangesAsync();
    }
}
    
