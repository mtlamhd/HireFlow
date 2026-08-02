using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Mvc.Models.EmployerView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Mvc.Controllers;

[Authorize(Roles = RoleConstants.EmployerRoleName)]
public class EmployerController : Controller
{
    private readonly IEmployerProfileService _employerProfileService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public EmployerController(
        IEmployerProfileService employerProfileService,
        IAttachmentService attachmentService, IUnitOfWork unitOfWork)
    {
        _employerProfileService = employerProfileService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return Guid.Parse(userIdClaim);
    }
    [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileDto = await _employerProfileService.GetMyProfileAsync(userId);
                
               
                var viewModel = new UpdateEmployerProfileViewModel
                {
                    FirstName = profileDto.FirstName ?? string.Empty,
                    LastName = profileDto.LastName ?? string.Empty,
                    Email = profileDto.Email ?? string.Empty,
                    NationalId = profileDto.NationalId ?? string.Empty,
                    BirthDate = profileDto.BirthDate ?? DateTime.UtcNow.AddYears(-25),
                    ProfileImageId = profileDto.ProfileImageId,
                    PhoneNumber = profileDto.Username
                };

                return View(viewModel);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateEmployerProfileViewModel model)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                
                var originalProfile = await _employerProfileService.GetMyProfileAsync(userId);
                model.PhoneNumber = originalProfile.Username;
                model.ProfileImageId = originalProfile.ProfileImageId;
                return View(model);
            }

            try
            {
                
                var updateDto = new UpdateEmployerProfileDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    NationalId = model.NationalId,
                    BirthDate = model.BirthDate
                };

                await _employerProfileService.UpdateMyProfileAsync(userId, updateDto);
                TempData["Message"] = "پروفایل شخصی شما با موفقیت به‌روزرسانی شد. ✨";
                return RedirectToAction(nameof(Profile));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var originalProfile = await _employerProfileService.GetMyProfileAsync(userId);
                model.PhoneNumber = originalProfile.Username;
                model.ProfileImageId = originalProfile.ProfileImageId;
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    throw new InvalidFilePayloadException("لطفاً یک فایل تصویری معتبر انتخاب کنید.");

                var userId = GetCurrentUserId();
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);

                var attachmentResult = await _attachmentService.UploadAsync(new UploadAttachmentDto
                {
                    FileName = imageFile.FileName,
                    ContentType = imageFile.ContentType,
                    Data = memoryStream.ToArray()
                });

                await _employerProfileService.SetMyProfileImageAsync(userId, attachmentResult.Id);
                TempData["Message"] = "عکس پروفایل شما با موفقیت به‌روزرسانی شد. 🖼️";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Profile));
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveProfileImage()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _employerProfileService.RemoveMyProfileImageAsync(userId);
                TempData["Message"] = "عکس پروفایل حذف شد.";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Profile));
        }
        [HttpGet]
        public async Task<IActionResult> GetAttachment(Guid id)
        {
            var attachment = await _unitOfWork.Attachments.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();

            return File(attachment.Data, attachment.ContentType);
        }
}