using HireFlow.Domain.Dtos.CompanyDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface ICompanyService
{
    Task<CompanyDetailsDto> GetMyCompanyAsync(Guid userId);
    
    Task UpdateMyCompanyAsync(Guid userId, UpdateCompanyDto dto);
    
    Task SetMyCompanyLogoAsync(Guid userId, Guid attachmentId);
    
    Task RemoveMyCompanyLogoAsync(Guid userId);
}