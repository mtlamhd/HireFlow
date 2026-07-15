using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<User> _userManager;

    public AdminService(UserManager<User> userManager)
    {
        _userManager = userManager;
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
}