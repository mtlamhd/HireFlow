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
            throw new Exception("The specified city does not exist.");

   
        var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            throw new Exception("The specified category does not exist.");
        
        
        var uniqueSkillIds = new List<Guid>();
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            uniqueSkillIds = dto.SkillIds.Distinct().ToList();
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new Exception("One or more skill IDs are invalid.");
        }

     
        var jobAd = await _unitOfWork.JobAds.CreateJobAdAsync(company.Id, dto, uniqueSkillIds);

        await _unitOfWork.SaveChangesAsync();

        
        var detailsDto = await _unitOfWork.JobAds.GetJobAdDetailsAsync(jobAd.Id);
        return detailsDto ?? throw new Exception("Error retrieving created job ad details.");
    }
    
    public async Task UpdateJobAdAsync(Guid userId, Guid jobAdId, UpdateJobAdDto dto)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

       
        var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId);
        if (!cityExists)
            throw new Exception("The specified city does not exist.");

        var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            throw new Exception("The specified category does not exist.");

        
        var uniqueSkillIds = new List<Guid>();
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            uniqueSkillIds = dto.SkillIds.Distinct().ToList();
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new Exception("One or more skill IDs are invalid.");
        }

      
        var isUpdated = await _unitOfWork.JobAds.UpdateJobAdAsync(jobAdId, dto, uniqueSkillIds, userId);
        if (!isUpdated)
            throw new Exception("Job ad not found.");

        
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteJobAdAsync(Guid userId, Guid jobAdId)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

        
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId, tracking: true);
        
        if (jobAd == null)
            throw new Exception("Job ad not found.");

        
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
            throw new Exception("Job ad not found.");

        return detailsDto;
    }
    
    public async Task DeactivateJobAdAsync(Guid userId, Guid jobAdId)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);
        
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId, tracking: true);
        if (jobAd == null)
            throw new Exception("Job ad not found.");

        jobAd.Deactivate(userId);
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task<Company> GetApprovedCompanyAndVerifyOwnershipAsync(Guid userId, Guid? jobAdId = null)
    {
        if (userId == Guid.Empty)
            throw new Exception("User ID cannot be empty.");

        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found.");

     
        if (!user.IsApproved)
            throw new Exception("Your employer account is not approved by system admin yet.");

       
        var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
        if (company == null)
            throw new Exception("No registered company found for this employer.");

      
        if (jobAdId.HasValue)
        {
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId.Value);
            if (jobAd == null)
                throw new Exception("The requested job ad does not exist.");

           
            if (jobAd.CompanyId != company.Id)
                throw new Exception("Access Denied. You do not own this job ad.");
        }

        return company;
    }
}