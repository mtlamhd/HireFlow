using HireFlow.Domain.Dtos.JobAdDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IJobAdService
{
   
    Task<JobAdDetailsDto> CreateJobAdAsync(Guid userId, CreateJobAdDto dto);

    
    Task UpdateJobAdAsync(Guid userId, Guid jobAdId, UpdateJobAdDto dto);

    
    Task DeleteJobAdAsync(Guid userId, Guid jobAdId);

   
    Task<List<JobAdSummaryDto>> GetMyCompanyJobAdsAsync(Guid userId);

   
    Task<JobAdDetailsDto> GetMyJobAdDetailsAsync(Guid userId, Guid jobAdId);

    Task DeactivateJobAdAsync(Guid userId, Guid jobAdId);
}