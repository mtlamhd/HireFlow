using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.EmailDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Mvc.Controllers;

[Authorize(Roles = RoleConstants.AdminRoleName)]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IAdminService adminService, IUnitOfWork unitOfWork)
        {
            _adminService = adminService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Employers));
        }

       
        private Guid GetCurrentAdminId()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                throw new ResourceAccessDeniedException("شناسه ادمین معتبر نیست یا احراز هویت نشده است.");
            }
            return adminId;
        }
        
        [HttpGet]
        public async Task<IActionResult> Employers()
        {
            try
            {
                var employers = await _adminService.GetAllEmployersAsync();
                return View(employers);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmployerDetails(Guid userId)
        {
            try
            {
                var details = await _adminService.GetEmployerDetailsAsync(userId);
                return View(details);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Employers));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(Guid userId)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.ApproveEmployerAsync(userId, adminId);
                TempData["Message"] = "حساب کاربری کارفرما با موفقیت تأیید شد و ایمیل اطلاع‌رسانی ارسال گردید. ✅";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Employers));
        }

        [HttpPost]
        public async Task<IActionResult> DisapproveEmployer(Guid userId)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.DisapproveEmployerAsync(userId, adminId);
                TempData["Message"] = "تأیید صلاحیت کارفرما لغو (رد) شد. 🚫";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Employers));
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
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return View(stats);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> JobSeekers()
        {
            try
            {
                var jobSeekers = await _adminService.GetAllJobSeekersAsync();
                return View(jobSeekers);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpGet]
        public async Task<IActionResult> JobSeekerDetails(Guid id)
        {
            try
            {
                var details = await _adminService.GetJobSeekerDetailsAsync(id);
                return View(details);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(JobSeekers));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActivateJobSeeker(Guid id)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.ActivateJobSeekerAsync(id, adminId);
                TempData["Message"] = "حساب کاربری کارجو با موفقیت فعال شد. 🟢";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(JobSeekers));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateJobSeeker(Guid id)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.DeactivateJobSeekerAsync(id, adminId);
                TempData["Message"] = "حساب کاربری کارجو غیرفعال شد. 🔴";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(JobSeekers));
        }
        [HttpGet]
                public async Task<IActionResult> JobAds()
                {
                    try
                    {
                        var jobAds = await _adminService.GetAllJobAdsForAdminAsync();
                        return View(jobAds);
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                        return RedirectToAction(nameof(Dashboard));
                    }
                }
        
                [HttpGet]
                public async Task<IActionResult> JobAdDetails(Guid id)
                {
                    try
                    {
                        var details = await _adminService.GetJobAdDetailsForAdminAsync(id);
                        return View(details);
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                        return RedirectToAction(nameof(JobAds));
                    }
                }
        
                [HttpPost]
                public async Task<IActionResult> ActivateJobAd(Guid id)
                {
                    try
                    {
                        var adminId = GetCurrentAdminId();
                        await _adminService.ActivateJobAdAsync(id, adminId);
                        TempData["Message"] = "آگهی استخدام با موفقیت فعال شد. 🟢";
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    return RedirectToAction(nameof(JobAds));
                }
        
                [HttpPost]
                public async Task<IActionResult> DeactivateJobAd(Guid id)
                {
                    try
                    {
                        var adminId = GetCurrentAdminId();
                        await _adminService.DeactivateJobAdAsync(id, adminId);
                        TempData["Message"] = "آگهی استخدام غیرفعال شد. 🔴";
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    return RedirectToAction(nameof(JobAds));
                }
        
                [HttpPost]
                public async Task<IActionResult> SoftDeleteJobAd(Guid id)
                {
                    try
                    {
                        var adminId = GetCurrentAdminId();
                        await _adminService.SoftDeleteJobAdAsync(id, adminId);
                        TempData["Message"] = "آگهی استخدام با موفقیت حذف (Soft Delete) شد. 🗑️";
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    return RedirectToAction(nameof(JobAds));
                }
        
                [HttpPost]
                public async Task<IActionResult> MakeFeatured(Guid id, DateTime featuredUntil)
                {
                    try
                    {
                        var adminId = GetCurrentAdminId();
                        await _adminService.MakeJobAdFeaturedAsync(id, featuredUntil, adminId);
                        TempData["Message"] = "آگهی مورد نظر با موفقیت ویژه (Featured) شد. ⭐";
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    return RedirectToAction(nameof(JobAds));
                }
        
                [HttpPost]
                public async Task<IActionResult> CancelFeatured(Guid id)
                {
                    try
                    {
                        var adminId = GetCurrentAdminId();
                        await _adminService.CancelJobAdFeaturedAsync(id, adminId);
                        TempData["Message"] = "وضعیت ویژه (Featured) آگهی لغو شد.";
                    }
                    catch (BaseAppException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    return RedirectToAction(nameof(JobAds));
                }
             [HttpGet]
        public async Task<IActionResult> EmailTemplates()
        {
            try
            {
                var templates = await _adminService.GetAllEmailTemplatesAsync();
                return View(templates);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEmailTemplate(Guid id)
        {
            try
            {
                var template = await _adminService.GetEmailTemplateByIdAsync(id);
                var updateDto = new UpdateEmailTemplateDto 
                { 
                    Subject = template.Subject, 
                    Body = template.Body 
                };
                ViewBag.TemplateId = template.Id;
                ViewBag.TypeName = template.TypeName; 
                return View(updateDto);
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(EmailTemplates));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditEmailTemplate(Guid id, UpdateEmailTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var template = await _adminService.GetEmailTemplateByIdAsync(id);
                ViewBag.TemplateId = id;
                ViewBag.TypeName = template?.TypeName ?? string.Empty;
                return View(dto);
            }

            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.UpdateEmailTemplateAsync(id, dto, adminId);
                TempData["Message"] = "قالب ایمیل با موفقیت به‌روزرسانی شد. ✉️";
                return RedirectToAction(nameof(EmailTemplates));
            }
            catch (BaseAppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var template = await _adminService.GetEmailTemplateByIdAsync(id);
                ViewBag.TemplateId = id;
                ViewBag.TypeName = template?.TypeName ?? string.Empty;
                return View(dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActivateEmailTemplate(Guid id)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.ActivateEmailTemplateAsync(id, adminId);
                TempData["Message"] = "ارسال ایمیل برای این رویداد فعال شد. 🟢";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(EmailTemplates));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateEmailTemplate(Guid id)
        {
            try
            {
                var adminId = GetCurrentAdminId();
                await _adminService.DeactivateEmailTemplateAsync(id, adminId);
                TempData["Message"] = "ارسال ایمیل برای این رویداد غیرفعال شد. 🔴";
            }
            catch (BaseAppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(EmailTemplates));
        }
    }
