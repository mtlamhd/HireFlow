using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IJobAdRepository : IGenericRepository<JobAd>
{
    Task<List<JobAdSummaryDto>> GetCompanyJobAdsAsync(Guid companyId);
    Task<JobAdDetailsDto?> GetJobAdDetailsAsync(Guid id);
    Task<JobAd> CreateJobAdAsync(Guid companyId, CreateJobAdDto dto, List<Guid> validSkillIds);
    Task<bool> UpdateJobAdAsync(Guid jobAdId, UpdateJobAdDto dto, List<Guid> validSkillIds, Guid requesterId);
    Task<List<PublicJobAdSummaryDto>> GetActiveJobAdsAsync(Paging paging);
    Task<PublicJobAdDetailsDto?> GetPublicJobAdDetailsAsync(Guid id);
    Task<List<PublicJobAdSummaryDto>> SearchActiveJobAdsAsync(JobAdSearchDto dto);
    Task<List<AdminJobAdSummaryDto>> GetAllJobAdsForAdminAsync();
    Task<AdminJobAdDetailsDto?> GetJobAdDetailsForAdminAsync(Guid id);
}