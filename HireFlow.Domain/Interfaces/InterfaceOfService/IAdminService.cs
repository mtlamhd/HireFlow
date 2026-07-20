using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IAdminService
{
    Task ApproveEmployerAsync(Guid userId, Guid requesterId);
    Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync();
}