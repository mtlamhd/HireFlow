using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;

public class JobAdService : IJobAdService
 {
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public JobAdService(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    public async Task<JobAdDetailsDto> CreateJobAdAsync(Guid userId, CreateJobAdDto dto)
    {
        var company = await GetApprovedCompanyAndVerifyOwnershipAsync(userId);
        
        var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId);
        if (!cityExists)
            throw new ItemNotFoundException("City", dto.CityId);

   
        var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            throw new ItemNotFoundException("Category", dto.CategoryId);
        
        
        var uniqueSkillIds = new List<Guid>();
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            uniqueSkillIds = dto.SkillIds.Distinct().ToList();
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new ItemNotFoundException("One or more specified skills were not found.");
        }

     
        var jobAd = await _unitOfWork.JobAds.CreateJobAdAsync(company.Id, dto, uniqueSkillIds);

        await _unitOfWork.SaveChangesAsync();

        
        var detailsDto = await _unitOfWork.JobAds.GetJobAdDetailsAsync(jobAd.Id);
        return detailsDto ?? throw new ItemNotFoundException("JobAd", jobAd.Id);
    }
    
    public async Task UpdateJobAdAsync(Guid userId, Guid jobAdId, UpdateJobAdDto dto)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

       
        var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId);
        if (!cityExists)
            throw new ItemNotFoundException("City", dto.CityId);

        var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            throw new ItemNotFoundException("Category", dto.CategoryId);

        
        var uniqueSkillIds = new List<Guid>();
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            uniqueSkillIds = dto.SkillIds.Distinct().ToList();
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new ItemNotFoundException("One or more specified skills were not found.");
        }

      
        var isUpdated = await _unitOfWork.JobAds.UpdateJobAdAsync(jobAdId, dto, uniqueSkillIds, userId);
        if (!isUpdated)
            throw new ItemNotFoundException("JobAd", jobAdId);

        
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteJobAdAsync(Guid userId, Guid jobAdId)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

        
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId, tracking: true);
        
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", jobAdId);

        
        _unitOfWork.JobAds.SoftDelete(jobAd, userId);

        
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<List<JobAdSummaryDto>> GetMyCompanyJobAdsAsync(Guid userId)
    {
        var company = await GetApprovedCompanyAndVerifyOwnershipAsync(userId);

        
        return await _unitOfWork.JobAds.GetCompanyJobAdsAsync(company.Id);
    }
    
    public async Task<JobAdDetailsDto> GetMyJobAdDetailsAsync(Guid userId, Guid jobAdId)
    {
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);
        
        var detailsDto = await _unitOfWork.JobAds.GetJobAdDetailsAsync(jobAdId);
        if (detailsDto == null)
            throw new ItemNotFoundException("JobAd", jobAdId);

        return detailsDto;
    }
    
    public async Task DeactivateJobAdAsync(Guid userId, Guid jobAdId)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);
        
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", jobAdId);

        jobAd.Deactivate(userId);
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task<Company> GetApprovedCompanyAndVerifyOwnershipAsync(Guid userId, Guid? jobAdId = null)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", userId);

     
        if (!user.IsApproved)
            throw new EmployerAccountNotApprovedException(userId);

       
        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
        if (company == null)
            throw new ItemNotFoundException($"No registered company found for employer with id '{userId}'.");

      
        if (jobAdId.HasValue)
        {
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId.Value);
            if (jobAd == null)
                throw new ItemNotFoundException("JobAd", jobAdId.Value);

           
            if (jobAd.CompanyId != company.Id)
                throw new ResourceAccessDeniedException("JobAd", jobAdId.Value);
        }

        return company;
    }
}