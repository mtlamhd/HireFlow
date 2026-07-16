using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.EmployerRoleName)] 
public class JobAdsController : ControllerBase
{
    private readonly IJobAdService _jobAdService;

    public JobAdsController(IJobAdService jobAdService)
    {
        _jobAdService = jobAdService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobAdDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _jobAdService.CreateJobAdAsync(userId, dto);
    
       
        return StatusCode(201, GenericResult<JobAdDetailsDto>.Success(result, "Job ad created successfully.", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobAdDto dto)
    {
        var userId = GetCurrentUserId();
        await _jobAdService.UpdateJobAdAsync(userId, id, dto);
        
        return Ok(GenericResult<Guid>.Success(id, "Job ad updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        await _jobAdService.DeleteJobAdAsync(userId, id);
        
        return Ok(GenericResult<Guid>.Success(id, "Job ad deleted successfully."));
    }

    [HttpGet("my-company")]
    public async Task<IActionResult> GetMyCompanyJobAds()
    {
        var userId = GetCurrentUserId();
        var jobAds = await _jobAdService.GetMyCompanyJobAdsAsync(userId);
        
        return Ok(GenericResult<List<JobAdSummaryDto>>.Success(jobAds));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        var jobAd = await _jobAdService.GetMyJobAdDetailsAsync(userId, id);
        
        return Ok(GenericResult<JobAdDetailsDto>.Success(jobAd));
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var userId = GetCurrentUserId();
        await _jobAdService.DeactivateJobAdAsync(userId, id);
        
        return Ok(GenericResult<Guid>.Success(id, "Job ad has been deactivated successfully."));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return Guid.Parse(userIdClaim);
    }
   
}