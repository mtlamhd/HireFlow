using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class JobAdRepository : GenericRepository<JobAd> , IJobAdRepository
{
    public JobAdRepository(AppDbContext context) : base(context)
    {
    }
     public async Task<List<JobAdSummaryDto>> GetCompanyJobAdsAsync(Guid companyId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobAdSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                CityName = j.City != null ? j.City.Name : string.Empty,
                CategoryName = j.Category != null ? j.Category.Name : string.Empty,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                IsActive = j.IsActive,
                ExpireAt = j.ExpireAt,
                ApplicationsCount = j.Requests.Count 
            })
            .ToListAsync();
    }

    
    public async Task<JobAdDetailsDto?> GetJobAdDetailsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new JobAdDetailsDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                CityId = j.CityId,
                CityName = j.City != null ? j.City.Name : string.Empty,
                CategoryId = j.CategoryId,
                CategoryName = j.Category != null ? j.Category.Name : string.Empty,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                IsActive = j.IsActive,
                ExpireAt = j.ExpireAt,
               
                Skills = j.JobAdSkills.Select(jas => new CategoryViewDto
                {
                    Id = jas.Skill.Id,
                    Name = jas.Skill.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    
    public async Task<JobAd> CreateJobAdAsync(Guid companyId, CreateJobAdDto dto, List<Guid> validSkillIds)
    {
       
        var jobAd = new JobAd(
            title: dto.Title,
            description: dto.Description,
            cityId: dto.CityId,
            categoryId: dto.CategoryId,
            companyId: companyId,
            employmentType: dto.EmploymentType,
            salary: dto.Salary
        );

      
        await _dbSet.AddAsync(jobAd);

       
        if (validSkillIds.Any())
        {
            var jobAdSkills = validSkillIds.Select(skillId => new JobAdSkill(jobAd.Id, skillId));
            await _context.JobAdSkills.AddRangeAsync(jobAdSkills);
        }

        return jobAd;
    }

    


    public async Task<bool> UpdateJobAdAsync(Guid jobAdId, UpdateJobAdDto dto, List<Guid> validSkillIds, Guid requesterId)
    {
       
        var jobAd = await _dbSet.FirstOrDefaultAsync(j => j.Id == jobAdId);
        if (jobAd == null)
        {
            return false;
        }

       
        jobAd.UpdateInfo(
            dto.Title,
            dto.Description,
            dto.CityId,
            dto.CategoryId,
            dto.EmploymentType,
            dto.Salary,
            requesterId
        );

       
        await _context.JobAdSkills
            .Where(jas => jas.JobAdId == jobAdId)
            .ExecuteDeleteAsync();

       
        if (validSkillIds.Any())
        {
            var newJobAdSkills = validSkillIds.Select(skillId => new JobAdSkill(jobAdId, skillId));
            await _context.JobAdSkills.AddRangeAsync(newJobAdSkills);
        }

        return true;
    }
    public async Task<List<PublicJobAdSummaryDto>> GetActiveJobAdsAsync(Paging paging)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Where(j => j.IsActive && j.ExpireAt > now)
            
            .OrderByDescending(j => j.IsFeatured && j.FeaturedUntil > now) 
            
            .ThenByDescending(j => j.HighlightExpireAt > now) 
            
            .ThenByDescending(j => j.CreatedAt)
            .Skip(paging.GetSkip())
            .Take(paging.PageSize)
            .Select(j => new PublicJobAdSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                CompanyName = j.Company.Name,
                CompanyLogoId = j.Company.LogoId,
                CityName = j.City.Name,
                ProvinceName = j.City.Province.Name,
                CategoryName = j.Category.Name,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                IsHighlighted = j.HighlightExpireAt > now,
                IsFeatured = j.IsFeatured && j.FeaturedUntil > now,
                CreatedAt = j.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<PublicJobAdDetailsDto?> GetPublicJobAdDetailsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new PublicJobAdDetailsDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                CityName = j.City.Name,
                ProvinceName = j.City.Province.Name,
                CategoryName = j.Category.Name,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                CreatedAt = j.CreatedAt,
                CompanyName = j.Company.Name,
                CompanyLogoId = j.Company.LogoId,
                IsActive = j.IsActive,
                ExpireAt = j.ExpireAt,
                Skills = j.JobAdSkills.Select(jas => new SkillViewDto
                {
                    Id = jas.Skill.Id,
                    Name = jas.Skill.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<PublicJobAdSummaryDto>> SearchActiveJobAdsAsync(JobAdSearchDto dto)
{
    var now = DateTime.UtcNow;
    
    var query = _dbSet.AsNoTracking()
        .Where(j => j.IsActive && j.ExpireAt > now);

    // اعمال فیلترهای پویا
    if (!string.IsNullOrWhiteSpace(dto.Title))
    {
        query = query.Where(j => j.Title.Contains(dto.Title));
    }

    if (dto.EmploymentType.HasValue)
    {
        query = query.Where(j => j.EmploymentType == dto.EmploymentType.Value);
    }

    if (dto.CityId.HasValue && dto.CityId.Value != Guid.Empty)
    {
        query = query.Where(j => j.CityId == dto.CityId.Value);
    }

    if (dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty)
    {
        query = query.Where(j => j.CategoryId == dto.CategoryId.Value);
    }

    if (dto.MinSalary.HasValue)
    {
        query = query.Where(j => j.Salary.HasValue && j.Salary.Value >= dto.MinSalary.Value);
    }

    if (dto.SkillIds != null && dto.SkillIds.Any())
    {
        var uniqueSkillIds = dto.SkillIds.Distinct().ToList();
        query = query.Where(j => j.JobAdSkills.Any(jas => uniqueSkillIds.Contains(jas.SkillId)));
    }
    
    query = query.OrderByDescending(j => j.IsFeatured && j.FeaturedUntil > now)
                 .ThenByDescending(j => j.HighlightExpireAt > now)            
                 .ThenByDescending(j => j.CreatedAt);                         

    query = query.Skip(dto.GetSkip()).Take(dto.PageSize); 

    return await query.Select(j => new PublicJobAdSummaryDto
    {
        Id = j.Id,
        Title = j.Title,
        CompanyName = j.Company.Name,
        CompanyLogoId = j.Company.LogoId,
        CityName = j.City.Name,
        ProvinceName = j.City.Province.Name,
        CategoryName = j.Category.Name,
        Salary = j.Salary,
        EmploymentType = j.EmploymentType,
        IsHighlighted = j.HighlightExpireAt > now,
        IsFeatured = j.IsFeatured && j.FeaturedUntil > now, 
        CreatedAt = j.CreatedAt
    }).ToListAsync();
}
    public async Task<List<AdminJobAdSummaryDto>> GetAllJobAdsForAdminAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new AdminJobAdSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                CompanyName = j.Company.Name,
                CityName = j.City.Name,
                CategoryName = j.Category.Name,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                IsActive = j.IsActive,
                IsFeatured = j.IsFeatured,
                FeaturedUntil = j.FeaturedUntil,
                ExpireAt = j.ExpireAt,
                CreatedAt = j.CreatedAt,
                ApplicationsCount = j.Requests.Count 
            })
            .ToListAsync();
    }

    public async Task<AdminJobAdDetailsDto?> GetJobAdDetailsForAdminAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new AdminJobAdDetailsDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Salary = j.Salary,
                EmploymentType = j.EmploymentType,
                IsActive = j.IsActive,
                ExpireAt = j.ExpireAt,
                CreatedAt = j.CreatedAt,
                HighlightExpireAt = j.HighlightExpireAt,
                IsHighlighted = j.HighlightExpireAt.HasValue && j.HighlightExpireAt.Value > DateTime.UtcNow,
                IsFeatured = j.IsFeatured,
                FeaturedUntil = j.FeaturedUntil,
                IsFeaturedActive = j.IsFeatured && j.FeaturedUntil.HasValue && j.FeaturedUntil.Value > DateTime.UtcNow,
                CompanyId = j.CompanyId,
                CompanyName = j.Company.Name,
                CompanyLogoId = j.Company.LogoId,
                CityName = j.City.Name,
                ProvinceName = j.City.Province.Name,
                CategoryName = j.Category.Name,
                Skills = j.JobAdSkills.Select(jas => new SkillViewDto
                {
                    Id = jas.Skill.Id,
                    Name = jas.Skill.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

} 
