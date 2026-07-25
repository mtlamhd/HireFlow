using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IEmployerProfileService
{
    Task<EmployerProfileDto> GetMyProfileAsync(Guid userId);

   
    Task UpdateMyProfileAsync(Guid userId, UpdateEmployerProfileDto dto);

   
    Task SetMyProfileImageAsync(Guid userId, Guid attachmentId);

   
    Task RemoveMyProfileImageAsync(Guid userId);
}