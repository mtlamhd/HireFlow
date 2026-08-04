using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
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
    }
