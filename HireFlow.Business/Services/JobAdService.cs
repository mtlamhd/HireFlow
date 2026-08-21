using System.Text.Json;
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
    private readonly ICacheService _cacheService;

    public JobAdService(IUnitOfWork unitOfWork, UserManager<User> userManager, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _cacheService = cacheService;
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
        
        await _cacheService.RemoveAsync("jobads:active:page:1:size:10");
        
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
        await _cacheService.RemoveAsync("jobads:active:page:1:size:10");
    }
    public async Task DeleteJobAdAsync(Guid userId, Guid jobAdId)
    {
       
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

        
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId, tracking: true);
        
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", jobAdId);

        
        _unitOfWork.JobAds.SoftDelete(jobAd, userId);

        
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveAsync("jobads:active:page:1:size:10");
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
        await _cacheService.RemoveAsync("jobads:active:page:1:size:10");
    }
    public async Task ActivateJobAdAsync(Guid userId, Guid jobAdId)
    {
        await GetApprovedCompanyAndVerifyOwnershipAsync(userId, jobAdId);

        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId,tracking: true);

        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", jobAdId);

        jobAd.Activate(userId);

        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveAsync("jobads:active:page:1:size:10");
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
    
    public async Task<List<PublicJobAdSummaryDto>> GetActiveJobAdsAsync(Paging paging)
    {
      
        if (paging == null)
            throw new InvalidRequestException("Paging parameters cannot be null.");
        
        string cacheKey = $"jobads:active:page:{paging.PageNumber}:size:{paging.PageSize}";
        var cachedData = await _cacheService.GetAsync<List<PublicJobAdSummaryDto>>(cacheKey);
    
       
        if (cachedData != null)
        {
            return cachedData; 
        }
        
        var jobAds =  await _unitOfWork.JobAds.GetActiveJobAdsAsync(paging);
        
        await _cacheService.SetAsync(cacheKey, jobAds, TimeSpan.FromMinutes(5));
        return jobAds;
    }
    public async Task<PublicJobAdDetailsDto> GetPublicJobAdDetailsAsync(Guid id)
    {
       
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

       
        var details = await _unitOfWork.JobAds.GetPublicJobAdDetailsAsync(id);
        
        if (details == null)
            throw new ItemNotFoundException("JobAd", id);

       
        if (!details.IsActive)
            throw new InvalidRequestException("This job ad is no longer active.");

        
        if (details.ExpireAt <= DateTime.UtcNow)
            throw new InvalidRequestException("This job ad has expired.");

        return details;
    }
    public async Task<List<PublicJobAdSummaryDto>> SearchActiveJobAdsAsync(JobAdSearchDto dto)
    {
        dto ??= new JobAdSearchDto();

        string cacheKey = $"jobads:search:{JsonSerializer.Serialize(dto)}";

       
        var cached = await _cacheService.GetAsync<List<PublicJobAdSummaryDto>>(cacheKey);
        if (cached is not null)
        {
            return cached;
        }

        
        if (dto.CityId.HasValue && dto.CityId.Value != Guid.Empty)
        {
            var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == dto.CityId.Value);
            if (!cityExists)
                throw new ItemNotFoundException("City", dto.CityId.Value);
        }

        if (dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty)
        {
            var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
            if (!categoryExists)
                throw new ItemNotFoundException("Category", dto.CategoryId.Value);
        }

        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            var uniqueSkillIds = dto.SkillIds.Distinct().ToList();
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new ItemNotFoundException("One or more specified skills were not found.");
        }

        
        var result = await _unitOfWork.JobAds.SearchActiveJobAdsAsync(dto);

        
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(3));

        return result;
    }
 }