using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public AdminService(UserManager<User> userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task ApproveEmployerAsync(Guid userId, Guid requesterId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            throw new ItemNotFoundException("User", userId);

        user.Approve(requesterId);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Employer Approval", errorMessage);
        }
    }

    public async Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync()
    {
        return await _userRepository.GetUnapprovedEmployersAsync();
    }

    public async Task<List<AdminJobSeekerSummaryDto>> GetAllJobSeekersAsync()
    {

        return await _userRepository.GetAllJobSeekersAsync(RoleConstants.JobSeekerRoleName);

    }

    public async Task<AdminJobSeekerDetailsDto> GetJobSeekerDetailsAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Seeker ID cannot be empty.");

        var detailsDto = await _userRepository.GetJobSeekerDetailsForAdminAsync(id);

        if (detailsDto == null)
            throw new ItemNotFoundException("JobSeeker", id);

        return detailsDto;
    }

    public async Task ActivateJobSeekerAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");


        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", id);


        user.Activate(requesterId);


        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Activate JobSeeker", errorMessage);
        }
    }

    public async Task DeactivateJobSeekerAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", id);


        user.Deactivate(requesterId);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Deactivate JobSeeker", errorMessage);
        }

    }
}