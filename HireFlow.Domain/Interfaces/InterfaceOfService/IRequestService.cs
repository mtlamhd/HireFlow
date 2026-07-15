using HireFlow.Domain.Dtos.RequestDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IRequestService
{
    Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid userId, Guid jobAdId);
    Task<RequestViewDto> GetRequestDetailsAsync(Guid userId, Guid requestId);
    Task ChangeRequestStatusAsync(Guid userId, Guid requestId, ChangeRequestStatusDto dto);
}