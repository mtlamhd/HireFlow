using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<CompanyDetailsDto?> GetCompanyDetailsByOwnerIdAsync(Guid ownerId);
    Task<bool> UpdateCompanyAndCategoriesAsync(Guid ownerId, UpdateCompanyDto dto, List<Guid> validCategoryIds);
    
}