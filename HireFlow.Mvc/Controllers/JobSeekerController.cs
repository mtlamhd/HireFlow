using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Mvc.Models.JobSeekerView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Mvc.Controllers
{
    [Authorize(Roles = RoleConstants.JobSeekerRoleName)]
    public class JobSeekerController : Controller
    {
        private readonly IJobSeekerProfileService _profileService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestService _requestService;

        
        
        
        public JobSeekerController(
            IJobSeekerProfileService profileService,
            IAttachmentService attachmentService, 
            IUnitOfWork unitOfWork,
            IRequestService requestService)
        {
            _profileService = profileService;
            _attachmentService = attachmentService;
            _unitOfWork = unitOfWork;
            _requestService = requestService;
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
                var profileDto = await _profileService.GetMyProfileAsync(userId);
                if (profileDto == null)
                    return NotFound();

                var viewModel = new JobSeekerProfileViewModel
                {
                    Id = profileDto.Id,
                    FirstName = profileDto.FirstName,
                    LastName = profileDto.LastName,
                    PhoneNumber = profileDto.PhoneNumber,
                    Email = profileDto.Email,
                    BirthDate = profileDto.BirthDate,
                    NationalId = profileDto.NationalId,
                    ProfileImageId = profileDto.ProfileImageId,
                    ResumeId = profileDto.ResumeId,
                    Skills = profileDto.Skills,
                    SkillIds = profileDto.Skills.Select(s => s.Id).ToList() 
                };

               
                ViewBag.AllSkills = await _unitOfWork.Skills.QueryAsync(s => true, new Paging { PageSize = 200 });

                return View(viewModel);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Profile(JobSeekerProfileViewModel model)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var originalProfile = await _profileService.GetMyProfileAsync(userId);
                if (originalProfile != null)
                {
                    model.PhoneNumber = originalProfile.PhoneNumber;
                    model.Skills = originalProfile.Skills;
                    model.ResumeId = originalProfile.ResumeId;
                    model.ProfileImageId = originalProfile.ProfileImageId;
                }
                ViewBag.AllSkills = await _unitOfWork.Skills.QueryAsync(s => true, new Paging { PageSize = 200 });
                return View(model);
            }

            try
            {
                var updateDto = new UpdateJobSeekerProfileDto
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Email = model.Email!,
                    BirthDate = model.BirthDate ?? DateTime.UtcNow,
                    NationalId = model.NationalId!,
                    SkillIds = model.SkillIds 
                };

                await _profileService.UpdateMyProfileAsync(userId, updateDto);
                TempData["Message"] = "پروفایل و مهارت‌های شما با موفقیت به‌روزرسانی شد. ✨";
                return RedirectToAction(nameof(Profile));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var originalProfile = await _profileService.GetMyProfileAsync(userId);
                if (originalProfile != null)
                {
                    model.PhoneNumber = originalProfile.PhoneNumber;
                    model.Skills = originalProfile.Skills;
                }
                ViewBag.AllSkills = await _unitOfWork.Skills.QueryAsync(s => true, new Paging { PageSize = 200 });
                return View(model);
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> UploadResume(IFormFile resumeFile)
        {
            try
            {
                if (resumeFile == null || resumeFile.Length == 0)
                    throw new InvalidFilePayloadException("لطفاً یک فایل PDF معتبر انتخاب کنید.");

                var extension = Path.GetExtension(resumeFile.FileName).ToLower();
                if (extension != ".pdf")
                    throw new InvalidFilePayloadException("فقط فایل‌های PDF برای رزومه مجاز هستند.");

                var userId = GetCurrentUserId();
                using var memoryStream = new MemoryStream();
                await resumeFile.CopyToAsync(memoryStream);

                var attachmentResult = await _attachmentService.UploadAsync(new UploadAttachmentDto
                {
                    FileName = resumeFile.FileName,
                    ContentType = resumeFile.ContentType,
                    Data = memoryStream.ToArray()
                });

                await _profileService.SetMyResumeAsync(userId, attachmentResult.Id);
                TempData["Message"] = "رزومه شما با موفقیت آپلود شد.";
                return RedirectToAction(nameof(Profile));
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> RemoveResume()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _profileService.RemoveMyResumeAsync(userId);
                TempData["Message"] = "رزومه شما با موفقیت حذف شد.";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Profile));
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

                await _profileService.SetMyProfileImageAsync(userId, attachmentResult.Id);
                TempData["Message"] = "عکس پروفایل شما با موفقیت آپلود شد.";
                return RedirectToAction(nameof(Profile));
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> RemoveProfileImage()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _profileService.RemoveMyProfileImageAsync(userId);
                TempData["Message"] = "عکس پروفایل شما با موفقیت حذف شد.";
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

       
        [HttpPost]
        public async Task<IActionResult> Apply(Guid jobAdId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _requestService.ApplyForJobAdAsync(userId, new ApplyJobAdDto { JobAdId = jobAdId });
                TempData["Message"] = "درخواست همکاری شما با موفقیت ارسال شد! 🚀";
            }
            catch (BaseAppException ex)
            {
                string errorMsg = ex.Message;
        
                if (errorMsg.Contains("resume", StringComparison.OrdinalIgnoreCase))
                {
                    errorMsg = "⚠️ برای ارسال درخواست، ابتدا باید رزومه PDF خود را در پنل کاربری آپلود کنید!";
                }
           
                else if (errorMsg.Contains("deactivated", StringComparison.OrdinalIgnoreCase) || errorMsg.Contains("deactivate", StringComparison.OrdinalIgnoreCase))
                {
                    errorMsg = "⚠️ حساب کاربری شما توسط ادمین غیرفعال شده است. امکان ارسال درخواست همکاری وجود ندارد.";
                }
                else if (ex is ConflictException || errorMsg.Contains("already submitted", StringComparison.OrdinalIgnoreCase))
                {
                    errorMsg = "⚠️ شما قبلاً برای این فرصت شغلی درخواست ارسال کرده‌اید!";
                }
        
                TempData["Error"] = errorMsg;
            }
            catch (Exception)
            {
                TempData["Error"] = "خطایی در ارسال درخواست رخ داد.";
            }

            return RedirectToAction("Details", "Home", new { id = jobAdId });
        }

       
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                var requests = await _requestService.GetJobSeekerRequestsAsync(userId);
                return View(requests);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> CancelRequest(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _requestService.CancelRequestAsync(userId, id);
                TempData["Message"] = "درخواست همکاری شما با موفقیت لغو شد.";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(MyRequests));
        }
    }
}
