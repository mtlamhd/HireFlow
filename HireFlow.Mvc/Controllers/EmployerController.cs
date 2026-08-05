using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Dtos.CompanyDto;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Mvc.Models.EmployerView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace HireFlow.Mvc.Controllers
{
    [Authorize(Roles = RoleConstants.EmployerRoleName)]
    public class EmployerController : Controller
    {
        private readonly IEmployerProfileService _employerProfileService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompanyService _companyService;
        private readonly IJobAdService _jobAdService;
        private readonly UserManager<User> _userManager;
        private readonly IRequestService _requestService;

        public EmployerController(
            IEmployerProfileService employerProfileService,
            IAttachmentService attachmentService, 
            IUnitOfWork unitOfWork, 
            UserManager<User> userManager, 
            ICompanyService companyService,
            IJobAdService jobAdService, IRequestService requestService)
        {
            _employerProfileService = employerProfileService;
            _attachmentService = attachmentService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _companyService = companyService;
            _jobAdService = jobAdService;
            _requestService = requestService;
        }

       
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new ResourceAccessDeniedException("کاربر احراز هویت نشده است.");

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
                ModelState.AddModelError(string.Empty, ex.Message);
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
        public async Task<IActionResult> Company()
        {
            try
            {
                var userId = GetCurrentUserId();
                var companyDto = await _companyService.GetMyCompanyAsync(userId);

                var viewModel = new UpdateCompanyViewModel
                {
                    Name = companyDto.Name,
                    Description = companyDto.Description ?? string.Empty,
                    Website = companyDto.Website ?? string.Empty,
                    Email = companyDto.Email ?? string.Empty,
                    PhoneNumber = companyDto.PhoneNumber ?? string.Empty,
                    Address = companyDto.Address ?? string.Empty,
                    CityId = companyDto.CityId,
                    LogoId = companyDto.LogoId,
                    CategoryIds = companyDto.Categories.Select(c => c.Id).ToList()
                };

                await FillCompanyViewBagsAsync(userId);
                return View(viewModel);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Company(UpdateCompanyViewModel model)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

          
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز توسط ادمین تایید نشده است. امکان ویرایش اطلاعات شرکت وجود ندارد.";
                return RedirectToAction(nameof(Company));
            }

           
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                var trimmed = model.Website.Trim();
                if (!trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                    !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    model.Website = "https://" + trimmed;
                }
                
                ModelState.Remove(nameof(model.Website));
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
            {
                await FillCompanyViewBagsAsync(userId);
                return View(model);
            }

            try
            {
                var dto = new UpdateCompanyDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    Website = model.Website,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CityId = model.CityId,
                    CategoryIds = model.CategoryIds
                };

                await _companyService.UpdateMyCompanyAsync(userId, dto);
                TempData["Message"] = "اطلاعات شرکت با موفقیت به‌روزرسانی شد. 🏢";
                return RedirectToAction(nameof(Company));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await FillCompanyViewBagsAsync(userId);
                return View(model);
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
                TempData["Message"] = "لوگوی شرکت با موفقیت آپلود شد. 🎨";
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

        private async Task FillCompanyViewBagsAsync(Guid userId)
        {
            ViewBag.Cities = await _unitOfWork.Cities.QueryAsync(c => true, new Paging { PageSize = 100 });
            ViewBag.Categories = await _unitOfWork.Categories.QueryAsync(c => true, new Paging { PageSize = 100 });

            var user = await _userManager.FindByIdAsync(userId.ToString());
            ViewBag.IsApproved = user?.IsApproved ?? false;
        }

        

        [HttpGet]
        public async Task<IActionResult> JobAds()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
          
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز توسط ادمین تایید نشده است. امکان دسترسی به مدیریت آگهی‌ها وجود ندارد.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var jobAds = await _jobAdService.GetMyCompanyJobAdsAsync(userId);
                ViewBag.IsApproved = user.IsApproved;
                return View(jobAds);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id);
                if (jobAd == null)
                    return NotFound();

                if (jobAd.IsActive)
                {
                    await _jobAdService.DeactivateJobAdAsync(userId, id);
                    TempData["Message"] = "آگهی استخدام با موفقیت غیرفعال شد. ⏸️";
                }
                else
                {
                    await _jobAdService.ActivateJobAdAsync(userId, id);
                    TempData["Message"] = "آگهی استخدام مجدداً فعال شد. 🟢";
                }
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(JobAds));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJobAd(Guid id)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                await _jobAdService.DeleteJobAdAsync(userId, id);
                TempData["Message"] = "آگهی استخدام با موفقیت حذف نرم‌افزاری شد. 🗑️";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(JobAds));
        }

        [HttpGet]
        public async Task<IActionResult> CreateJobAd()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            await FillJobAdViewBagsAsync();
            return View(new CreateJobAdViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobAd(CreateJobAdViewModel model)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید نشده است.";
                return RedirectToAction(nameof(Company));
            }

            if (!ModelState.IsValid)
            {
                await FillJobAdViewBagsAsync();
                return View(model);
            }

            try
            {
                var dto = new CreateJobAdDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    CityId = model.CityId.Value,                   
                    CategoryId = model.CategoryId.Value,         
                    Salary = model.Salary,
                    EmploymentType = model.EmploymentType.Value,  
                    SkillIds = model.SkillIds
                };

                await _jobAdService.CreateJobAdAsync(userId, dto);
                TempData["Message"] = "فرصت شغلی جدید با موفقیت منتشر شد. 🚀";
                return RedirectToAction(nameof(JobAds));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await FillJobAdViewBagsAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditJobAd(Guid id)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var details = await _jobAdService.GetMyJobAdDetailsAsync(userId, id);

                var viewModel = new EditJobAdViewModel
                {
                    Id = details.Id,
                    Title = details.Title,
                    Description = details.Description,
                    CityId = details.CityId,
                    CategoryId = details.CategoryId,
                    Salary = details.Salary,
                    EmploymentType = details.EmploymentType,
                    SkillIds = details.Skills.Select(s => s.Id).ToList()
                };

                await FillJobAdViewBagsAsync();
                return View(viewModel);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(JobAds));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditJobAd(EditJobAdViewModel model)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            if (!ModelState.IsValid)
            {
                await FillJobAdViewBagsAsync();
                return View(model);
            }

            try
            {
                var dto = new UpdateJobAdDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    CityId = model.CityId.Value,                 
                    CategoryId = model.CategoryId.Value,          
                    Salary = model.Salary,
                    EmploymentType = model.EmploymentType.Value, 
                    SkillIds = model.SkillIds
                };

                await _jobAdService.UpdateJobAdAsync(userId, model.Id, dto);
                TempData["Message"] = "تغییرات فرصت شغلی با موفقیت ثبت شد. ✨";
                return RedirectToAction(nameof(JobAds));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await FillJobAdViewBagsAsync();
                return View(model);
            }
        }

        private async Task FillJobAdViewBagsAsync()
        {
            ViewBag.Cities = await _unitOfWork.Cities.QueryAsync(c => true, new Paging { PageSize = 100 });
            ViewBag.Categories = await _unitOfWork.Categories.QueryAsync(c => true, new Paging { PageSize = 100 });
            ViewBag.Skills = await _unitOfWork.Skills.QueryAsync(s => true, new Paging { PageSize = 200 });

            ViewBag.EmploymentTypes = Enum.GetValues(typeof(EmploymentTypeEnum))
                .Cast<EmploymentTypeEnum>()
                .Select(e => new { Value = (int)e, Name = GetEmploymentTypeName(e) })
                .ToList();
        }

        private static string GetEmploymentTypeName(EmploymentTypeEnum type)
        {
            return type switch
            {
                EmploymentTypeEnum.FullTime => "تمام‌وقت",
                EmploymentTypeEnum.PartTime => "پاره‌وقت",
                EmploymentTypeEnum.Contract => "قراردادی",
                EmploymentTypeEnum.Internship => "کارآموزی",
                EmploymentTypeEnum.Remote => "دورکاری",
                _ => type.ToString()
            };
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
        public async Task<IActionResult> Requests(Guid jobAdId)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var requests = await _requestService.GetJobAdRequestsAsync(userId, jobAdId);
                ViewBag.JobAdId = jobAdId; 
                return View(requests);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(JobAds));
            }
        }
        [HttpGet]
        public async Task<IActionResult> RequestDetails(Guid id)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var requestDetails = await _requestService.GetRequestDetailsAsync(userId, id);
                return View(requestDetails);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(JobAds));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid requestId, Guid jobAdId, RequestStatusEnum newStatus)
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "⚠️ حساب کاربری شما هنوز تایید صلاحیت نشده است.";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                var changeDto = new ChangeRequestStatusDto { NewStatus = newStatus };
                await _requestService.ChangeRequestStatusAsync(userId, requestId, changeDto);
                
                TempData["Message"] = "وضعیت درخواست همکاری با موفقیت به‌روزرسانی شد و ایمیل اطلاع‌رسانی برای کارجو ارسال گردید. ✉️";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "خطایی در تغییر وضعیت درخواست رخ داد.";
            }

           
            return RedirectToAction(nameof(RequestDetails), new { id = requestId });
        }
    }
}