using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class EmployerProfileService : IEmployerProfileService
{
    private readonly IUserRepository _userRepository;

    public EmployerProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

   
    public async Task<EmployerProfileDto> GetMyProfileAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var profileDto = await _userRepository.GetEmployerProfileByIdAsync(userId);

        if (profileDto == null)
            throw new ItemNotFoundException("User", userId);

        return profileDto;
    }

   
    public async Task UpdateMyProfileAsync(Guid userId, UpdateEmployerProfileDto dto)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        if (dto == null)
            throw new InvalidRequestException("Profile data cannot be null.");

        
        var isUpdated = await _userRepository.UpdateEmployerProfileAsync(userId, dto, userId);

        if (!isUpdated)
            throw new ItemNotFoundException("User", userId);
    }

   
    public async Task SetMyProfileImageAsync(Guid userId, Guid attachmentId)
    {
        if (userId == Guid.Empty || attachmentId == Guid.Empty)
            throw new InvalidRequestException("User ID and Attachment ID cannot be empty.");

       
        var isUpdated = await _userRepository.SetUserProfileImageAsync(userId, attachmentId, userId);

        if (!isUpdated)
            throw new ItemNotFoundException("User", userId);
    }

  
    public async Task RemoveMyProfileImageAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        
        var isRemoved = await _userRepository.RemoveUserProfileImageAsync(userId, userId);

        if (!isRemoved)
            throw new ItemNotFoundException("User", userId);
    }
}