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
    
}