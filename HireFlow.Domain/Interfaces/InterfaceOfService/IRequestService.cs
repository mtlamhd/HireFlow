using HireFlow.Domain.Dtos.RequestDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IRequestService
{
    Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid userId, Guid jobAdId);
    Task<RequestViewDto> GetRequestDetailsAsync(Guid userId, Guid requestId);
    Task ChangeRequestStatusAsync(Guid userId, Guid requestId, ChangeRequestStatusDto dto);
    Task ApplyForJobAdAsync(Guid userId, ApplyJobAdDto dto);
    Task<List<JobSeekerRequestSummaryDto>> GetJobSeekerRequestsAsync(Guid userId);
    Task<JobSeekerRequestDetailsDto> GetJobSeekerRequestDetailsAsync(Guid userId, Guid requestId);
    Task CancelRequestAsync(Guid userId, Guid requestId);
}