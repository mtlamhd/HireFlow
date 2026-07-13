using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.AdminRoleName)]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("approve-employer/{userId}")]
    public async Task<IActionResult> ApproveEmployer(Guid userId)
    {
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null)
            return Unauthorized();

        var adminId = Guid.Parse(adminIdClaim);

        await _adminService.ApproveEmployerAsync(userId, adminId);
        
        return Ok(new { message = "Employer approved successfully." });
    }
}