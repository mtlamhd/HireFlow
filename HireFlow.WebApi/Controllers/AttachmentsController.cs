using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentsController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Download(Guid id)
    {
       
        var fileDto = await _attachmentService.DownloadAsync(id);
        
        return File(fileDto.Data, fileDto.ContentType, fileDto.FileName);
    }
}