using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Repositories;

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
                LogoId = c.LogoId,
                CityId = c.CityId,
                CityName = c.City != null ? c.City.Name : null,
                ProvinceId = c.City != null ? c.City.ProvinceId : null,
                ProvinceName = (c.City != null && c.City.Province != null) ? c.City.Province.Name : null,
                Categories = c.CompanyCategories.Select(cc => new CategoryViewDto
                {
                    Id = cc.Category.Id,
                    Name = cc.Category.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> UpdateCompanyAndCategoriesAsync(Guid ownerId, UpdateCompanyDto dto, List<Guid> validCategoryIds)
    {
        
        var company = await _dbSet.FirstOrDefaultAsync(c => c.OwnerId == ownerId);
        
        if (company == null)
        {
            return false;
        }

        company.UpdateInfo(
            dto.Name,
            dto.Description,
            dto.Website,
            dto.Email,
            dto.PhoneNumber,
            dto.Address,
            dto.CityId,
            ownerId
        );

        
        await _context.CompanyCategories
            .Where(cc => cc.CompanyId == company.Id)
            .ExecuteDeleteAsync();

        
        if (validCategoryIds.Any())
        {
            var newCompanyCategories = validCategoryIds.Select(catId => new CompanyCategory(company.Id, catId));
            await _context.CompanyCategories.AddRangeAsync(newCompanyCategories);
        }

        return true; 
    }
    
}