using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IAdminService
{
    Task ApproveEmployerAsync(Guid userId, Guid requesterId);
    Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync();
    Task<List<AdminJobSeekerSummaryDto>> GetAllJobSeekersAsync();
    Task<AdminJobSeekerDetailsDto> GetJobSeekerDetailsAsync(Guid id);
    Task ActivateJobSeekerAsync(Guid id, Guid requesterId);
    Task DeactivateJobSeekerAsync(Guid id, Guid requesterId);
    Task<List<AdminJobAdSummaryDto>> GetAllJobAdsForAdminAsync();
    Task<AdminJobAdDetailsDto> GetJobAdDetailsForAdminAsync(Guid id);
    
    Task ActivateJobAdAsync(Guid id, Guid requesterId);
    
    Task DeactivateJobAdAsync(Guid id, Guid requesterId);
    
    Task SoftDeleteJobAdAsync(Guid id, Guid requesterId);
    Task MakeJobAdFeaturedAsync(Guid id, DateTime expiresAt, Guid requesterId);
    
    Task CancelJobAdFeaturedAsync(Guid id, Guid requesterId);
    
    Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
    
}