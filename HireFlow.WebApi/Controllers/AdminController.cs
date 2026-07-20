using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
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
    [HttpGet("pending-employers")]
    public async Task<IActionResult> GetPendingEmployers()
    {
        var employers = await _adminService.GetUnapprovedEmployersAsync();

        return Ok(
            GenericResult<List<PendingEmployerDto>>.Success(
                employers,
                "Pending employers retrieved successfully."
            ));
    }

    [HttpPost("approve-employer/{userId}")]
    public async Task<IActionResult> ApproveEmployer(Guid userId)
    {
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null)
        {
           
            throw new ResourceAccessDeniedException("Admin identity claim was not found.");
        }

        var adminId = Guid.Parse(adminIdClaim);

        await _adminService.ApproveEmployerAsync(userId, adminId);
        
       
        return Ok(Result.Success("Employer approved successfully."));
    }
   
}