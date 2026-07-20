using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IJobSeekerProfileService
{
    Task<JobSeekerProfileDto> GetMyProfileAsync(Guid userId);
    Task UpdateMyProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto);
    Task SetMyResumeAsync(Guid userId, Guid attachmentId);
    Task RemoveMyResumeAsync(Guid userId);
    
    Task SetMyProfileImageAsync(Guid userId, Guid attachmentId);
    Task RemoveMyProfileImageAsync(Guid userId);
}