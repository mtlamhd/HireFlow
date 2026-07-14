using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.EmployerRoleName)]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("my-company")]
    public async Task<IActionResult> GetMyCompany()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var company = await _companyService.GetMyCompanyAsync(userId);
        
        return Ok(company);
    }
                                     
    [HttpPut("my-company")]
    public async Task<IActionResult> UpdateMyCompany([FromBody] UpdateCompanyDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        await _companyService.UpdateMyCompanyAsync(userId, dto);
        
        return Ok(new { message = "Company updated successfully." });
    }
}