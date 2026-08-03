using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Mvc.Models.EmployerView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Mvc.Controllers;

[Authorize(Roles = RoleConstants.EmployerRoleName)]
public class EmployerController : Controller
{
    private readonly IEmployerProfileService _employerProfileService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICompanyService _companyService;
    private readonly UserManager<User> _userManager;

    public EmployerController(
        IEmployerProfileService employerProfileService,
        IAttachmentService attachmentService, IUnitOfWork unitOfWork, UserManager<User> userManager, ICompanyService companyService)
    {
        _employerProfileService = employerProfileService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _companyService = companyService;
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
        [HttpGet]
        public async Task<IActionResult> Company()
        {
            try
            {
                var userId = GetCurrentUserId();
                var company = await _companyService.GetMyCompanyAsync(userId);

               
                ViewBag.Cities = await _unitOfWork.Cities.QueryAsync(c => true, new Paging { PageSize = 100 });
                ViewBag.Categories = await _unitOfWork.Categories.QueryAsync(c => true, new Paging { PageSize = 100 });

               
                var user = await _userManager.FindByIdAsync(userId.ToString());
                ViewBag.IsApproved = user?.IsApproved ?? false;

                return View(company);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

      
        [HttpPost]
        public async Task<IActionResult> Company(UpdateCompanyDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

           
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز توسط ادمین تایید نشده است. امکان ویرایش اطلاعات شرکت وجود ندارد.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                await _companyService.UpdateMyCompanyAsync(userId, dto);
                TempData["Message"] = "اطلاعات شرکت با موفقیت به‌روزرسانی شد. 🏢";
                return RedirectToAction(nameof(Company));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var company = await _companyService.GetMyCompanyAsync(userId);
                
                ViewBag.Cities = await _unitOfWork.Cities.QueryAsync(c => true, new Paging { PageSize = 100 });
                ViewBag.Categories = await _unitOfWork.Categories.QueryAsync(c => true, new Paging { PageSize = 100 });
                ViewBag.IsApproved = user.IsApproved;

                return View(company);
            }
        }

      
        [HttpPost]
        public async Task<IActionResult> UploadLogo(IFormFile logoFile)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید نشده است و نمی‌توانید لوگو آپلود کنید.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                if (logoFile == null || logoFile.Length == 0)
                    throw new InvalidFilePayloadException("لطفاً یک تصویر معتبر برای لوگو انتخاب کنید.");

                using var memoryStream = new MemoryStream();
                await logoFile.CopyToAsync(memoryStream);

                var attachmentResult = await _attachmentService.UploadAsync(new UploadAttachmentDto
                {
                    FileName = logoFile.FileName,
                    ContentType = logoFile.ContentType,
                    Data = memoryStream.ToArray()
                });

                await _companyService.SetMyCompanyLogoAsync(userId, attachmentResult.Id);
                TempData["Message"] = "لوگوی شرکت با موفقیت آپلود شد.";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Company));
        }

     
        [HttpPost]
        public async Task<IActionResult> RemoveLogo()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                await _companyService.RemoveMyCompanyLogoAsync(userId);
                TempData["Message"] = "لوگوی شرکت حذف شد.";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Company));
        }
}