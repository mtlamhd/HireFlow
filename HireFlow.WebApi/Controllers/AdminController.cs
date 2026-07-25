using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AdminDto;
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
    
    [HttpGet("job-seekers")]
    public async Task<IActionResult> GetAllJobSeekers()
    {
        var jobSeekers = await _adminService.GetAllJobSeekersAsync();

        return Ok(GenericResult<List<AdminJobSeekerSummaryDto>>.Success(jobSeekers));
    }

    
    [HttpGet("job-seekers/{id}")]
    public async Task<IActionResult> GetJobSeekerDetails(Guid id)
    {
        var details = await _adminService.GetJobSeekerDetailsAsync(id);

        return Ok(GenericResult<AdminJobSeekerDetailsDto>.Success(details));
    }
    
    [HttpPost("job-seekers/{id}/activate")]
    public async Task<IActionResult> ActivateJobSeeker(Guid id)
    {
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null)
        {
            throw new ResourceAccessDeniedException("Admin identity claim was not found.");
        }

        var adminId = Guid.Parse(adminIdClaim);

        await _adminService.ActivateJobSeekerAsync(id, adminId);

        return Ok(Result.Success("Job seeker account has been activated successfully."));
    }
    [HttpPost("job-seekers/{id}/deactivate")]
    public async Task<IActionResult> DeactivateJobSeeker(Guid id)
    {
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null)
        {
            throw new ResourceAccessDeniedException("Admin identity claim was not found.");
        }

        var adminId = Guid.Parse(adminIdClaim);

        await _adminService.DeactivateJobSeekerAsync(id, adminId);

        return Ok(Result.Success("Job seeker account has been deactivated successfully."));
    }
}