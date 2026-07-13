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
            throw new Exception("User not found.");

        user.Approve(requesterId);

        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            throw new Exception("Failed to approve employer.");
    }
}