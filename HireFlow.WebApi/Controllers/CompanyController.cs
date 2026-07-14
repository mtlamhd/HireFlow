using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.AttachmentDto;
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
    private readonly IAttachmentService _attachmentService;

   
    public CompanyController(
        ICompanyService companyService, 
        IAttachmentService attachmentService)
    {
        _companyService = companyService;
        _attachmentService = attachmentService;
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

    [HttpPost("my-company/logo")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
      
        if (file == null || file.Length == 0)
            return BadRequest("Please select a valid image file.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

       
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

      
        var uploadDto = new UploadAttachmentDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Data = fileBytes
        };

       
        var attachmentResult = await _attachmentService.UploadAsync(uploadDto);

       
        await _companyService.SetMyCompanyLogoAsync(userId, attachmentResult.Id);

        return Ok(new { message = "Company logo uploaded successfully.", logoId = attachmentResult.Id });
    }

   
    [HttpDelete("my-company/logo")]
    public async Task<IActionResult> RemoveLogo()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

       
        await _companyService.RemoveMyCompanyLogoAsync(userId);

        return Ok(new { message = "Company logo removed successfully." });
    }
}