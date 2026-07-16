using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IUserRepository
{
    Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync();
}