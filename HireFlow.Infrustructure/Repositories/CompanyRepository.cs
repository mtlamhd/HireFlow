using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class CompanyRepository : GenericRepository<Company> , ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<CompanyDetailsDto?> GetCompanyDetailsByOwnerIdAsync(Guid ownerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.OwnerId == ownerId)
            .Select(c => new CompanyDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Website = c.Website,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                LogoId = c.LogoId
            })
            .FirstOrDefaultAsync();
    }
}