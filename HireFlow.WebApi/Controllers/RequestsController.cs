using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
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
        var userId = GetCurrentUserId();
        var requests = await _requestService.GetJobAdRequestsAsync(userId, jobAdId);
        
        return Ok(GenericResult<List<RequestSummaryDto>>.Success(requests));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequestDetails(Guid id)
    {
        var userId = GetCurrentUserId();
        var requestDetails = await _requestService.GetRequestDetailsAsync(userId, id);
        
        return Ok(GenericResult<RequestViewDto>.Success(requestDetails));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeRequestStatusDto dto)
    {
        var userId = GetCurrentUserId();
        await _requestService.ChangeRequestStatusAsync(userId, id, dto);
        
        return Ok(GenericResult<Guid>.Success(id, "Request status updated successfully."));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return Guid.Parse(userIdClaim);
    }
}