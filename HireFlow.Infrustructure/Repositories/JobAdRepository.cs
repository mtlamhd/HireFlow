using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Dtos.JobAdDto;
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
}
