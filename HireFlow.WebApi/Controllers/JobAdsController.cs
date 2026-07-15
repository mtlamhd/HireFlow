using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
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
        try
        {
            var userId = GetCurrentUserId();
            var result = await _jobAdService.CreateJobAdAsync(userId, dto);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobAdDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _jobAdService.UpdateJobAdAsync(userId, id, dto);
            
            return Ok(new { message = "Job ad updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _jobAdService.DeleteJobAdAsync(userId, id);
            
            return Ok(new { message = "Job ad deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   
    [HttpGet("my-company")]
    public async Task<IActionResult> GetMyCompanyJobAds()
    {
        try
        {
            var userId = GetCurrentUserId();
            var jobAds = await _jobAdService.GetMyCompanyJobAdsAsync(userId);
            
            return Ok(jobAds);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var jobAd = await _jobAdService.GetMyJobAdDetailsAsync(userId, id);
            
            return Ok(jobAd);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    
    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _jobAdService.DeactivateJobAdAsync(userId, id);
            
            return Ok(new { message = "Job ad has been deactivated successfully." });
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