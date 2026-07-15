using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IRequestRepository : IGenericRepository<Request>
{
    Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid jobAdId);
    Task<RequestViewDto?> GetRequestDetailsAsync(Guid id);
    
}