using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.UserDto;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IUserRepository
{
    Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync();
    Task<JobSeekerProfileDto?> GetProfileByIdAsync(Guid userId);
    Task<bool> UpdateMyProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto, List<Guid> validSkillIds, Guid requesterId);
    
    Task<bool> SetUserResumeAsync(Guid userId, Guid attachmentId, Guid requesterId);
    Task<bool> RemoveUserResumeAsync(Guid userId, Guid requesterId);
    
    Task<bool> SetUserProfileImageAsync(Guid userId, Guid attachmentId, Guid requesterId);
    Task<bool> RemoveUserProfileImageAsync(Guid userId, Guid requesterId);
    
    Task<List<AdminJobSeekerSummaryDto>> GetAllJobSeekersAsync(string roleName);
    Task<AdminJobSeekerDetailsDto?> GetJobSeekerDetailsForAdminAsync(Guid id);
}