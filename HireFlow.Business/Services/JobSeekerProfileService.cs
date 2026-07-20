using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;


public class JobSeekerProfileService : IJobSeekerProfileService
{
    private readonly IUserRepository _userRepository; 
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttachmentRepository _attachmentRepository;

    public JobSeekerProfileService(
        IUserRepository userRepository,
        UserManager<User> userManager, IUnitOfWork unitOfWork, IAttachmentRepository attachmentRepository)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _attachmentRepository = attachmentRepository;
    }
    public async Task<JobSeekerProfileDto> GetMyProfileAsync(Guid userId)
    {
      
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");
       
        var profileDto = await _userRepository.GetProfileByIdAsync(userId);
        
        if (profileDto == null)
            throw new ItemNotFoundException("User", userId);

        return profileDto;
    }
    public async Task UpdateMyProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto)
    {
       
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        if (dto == null)
            throw new InvalidRequestException("Profile data cannot be null.");

        
        var uniqueSkillIds = new List<Guid>();
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            uniqueSkillIds = dto.SkillIds.Distinct().ToList();
        
           
            var validSkillsCount = await _unitOfWork.Skills.CountAsync(s => uniqueSkillIds.Contains(s.Id));

            
            if (validSkillsCount != uniqueSkillIds.Count)
                throw new ItemNotFoundException("One or more specified skills were not found.");
        }

       
        var isUpdated = await _userRepository.UpdateMyProfileAsync(userId, dto, uniqueSkillIds, userId);
    
        
        if (!isUpdated)
            throw new ItemNotFoundException("User", userId);
    }
    
    public async Task SetMyResumeAsync(Guid userId, Guid attachmentId)
    {
        if (userId == Guid.Empty || attachmentId == Guid.Empty)
            throw new InvalidRequestException("User ID and Attachment ID cannot be empty.");

        var isUpdated = await _userRepository.SetUserResumeAsync(userId, attachmentId, userId);
        if (!isUpdated)
            throw new ItemNotFoundException("User", userId);
    }

   
    public async Task RemoveMyResumeAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var isRemoved = await _userRepository.RemoveUserResumeAsync(userId, userId);
        if (!isRemoved)
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
