using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.EmployerRoleName)]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }

   
    [HttpGet("by-job-ad/{jobAdId}")]
    public async Task<IActionResult> GetRequestsByJobAd(Guid jobAdId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var requests = await _requestService.GetJobAdRequestsAsync(userId, jobAdId);
            
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequestDetails(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var requestDetails = await _requestService.GetRequestDetailsAsync(userId, id);
            
            return Ok(requestDetails);
        }
        catch (Exception ex)
        {
           
            return NotFound(new { message = ex.Message });
        }
    }

    
    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeRequestStatusDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _requestService.ChangeRequestStatusAsync(userId, id, dto);
            
            return Ok(new { message = "Request status updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return Guid.Parse(userIdClaim);
    }
   
}