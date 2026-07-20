using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicJobAdsController : ControllerBase
{
    private readonly IJobAdService _jobAdService;

    public PublicJobAdsController(IJobAdService jobAdService)
    {
        _jobAdService = jobAdService;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetActiveJobAds([FromQuery] Paging paging)
    {
        
        paging ??= new Paging();

        var jobAds = await _jobAdService.GetActiveJobAdsAsync(paging);

        return Ok(GenericResult<List<PublicJobAdSummaryDto>>.Success(jobAds));
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchJobAds([FromQuery] JobAdSearchDto dto)
    {
        dto ??= new JobAdSearchDto();

        var searchResults = await _jobAdService.SearchActiveJobAdsAsync(dto);

        return Ok(GenericResult<List<PublicJobAdSummaryDto>>.Success(searchResults));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobAdDetails(Guid id)
    {
        var jobAdDetails = await _jobAdService.GetPublicJobAdDetailsAsync(id);

        return Ok(GenericResult<PublicJobAdDetailsDto>.Success(jobAdDetails));
    }
}