namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IAdminService
{
    Task ApproveEmployerAsync(Guid userId, Guid requesterId);
}