using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.WebApi.ResultPaterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;


    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.EmployerRoleName)] 
    public class EmployerProfileController : ControllerBase
    {
        private readonly IEmployerProfileService _profileService;
        private readonly IAttachmentService _attachmentService;

        public EmployerProfileController(
            IEmployerProfileService profileService,
            IAttachmentService attachmentService)
        {
            _profileService = profileService;
            _attachmentService = attachmentService;
        }

      
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);

            return Ok(GenericResult<EmployerProfileDto>.Success(profile));
        }

      
        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateEmployerProfileDto dto)
        {
            var userId = GetCurrentUserId();
            await _profileService.UpdateMyProfileAsync(userId, dto);

            return Ok(GenericResult<bool>.Success(true, "Profile updated successfully."));
        }

       
        [HttpPost("my-profile/image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidFilePayloadException("Please select a valid image file.");

            var userId = GetCurrentUserId();
            
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

           
            await _profileService.SetMyProfileImageAsync(userId, attachmentResult.Id);

            return Ok(GenericResult<Guid>.Success(attachmentResult.Id, "Profile image uploaded successfully."));
        }

        
        [HttpDelete("my-profile/image")]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var userId = GetCurrentUserId();
            await _profileService.RemoveMyProfileImageAsync(userId);

            return Ok(GenericResult<bool>.Success(true, "Profile image removed successfully."));
        }

    
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(userIdClaim);
        }
    }
