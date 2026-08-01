using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AuthenticationDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Mvc.Models.AuthView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly IAuthenticationService _authService; 
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthController(
        IAuthenticationService authService,
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        _authService = authService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

   
    [HttpGet]
    public IActionResult Login() => View();

   
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            ModelState.AddModelError("", "کاربری با این مشخصات یافت نشد.");
            return View(model);
        }

      
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(RoleConstants.EmployerRoleName) && !user.IsApproved)
        {
            ModelState.AddModelError("", "حساب کاربری کارفرمایی شما هنوز توسط ادمین تایید نشده است.");
            return View(model);
        }

      
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است.");
            return View(model);
        }

       
        if (roles.Contains(RoleConstants.AdminRoleName))
            return RedirectToAction("Index", "Admin");
        
        if (roles.Contains(RoleConstants.EmployerRoleName))
            return RedirectToAction("Index", "Employer");

        return RedirectToAction("Index", "Home");
    }

   
    [HttpGet]
    public IActionResult RegisterJobSeeker() => View();

    
   
    [HttpPost]
    public async Task<IActionResult> RegisterJobSeeker(RegisterJobSeekerViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var dto = new RegisterJobSeekerDto { Username = model.Username, Password = model.Password };
            
           
            var result = await _authService.RegisterJobSeekerAsync(dto);

           
            var user = await _userManager.FindByIdAsync(result.Id.ToString());
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (ConflictException)
        {
            ModelState.AddModelError("", "این شماره موبایل قبلاً در سیستم ثبت‌نام کرده است.");
            return View(model);
        }
        catch (UserRegistrationException)
        {
           
            ModelState.AddModelError("", "خطا در ثبت‌نام: رمز عبور باید حداقل ۶ کاراکتر و شامل حروف و اعداد باشد.");
            return View(model);
        }
        catch (BaseAppException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
    
    [HttpGet]
    public IActionResult RegisterEmployer() => View();
    
    [HttpPost]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var dto = new RegisterEmployerDto 
            { 
                Username = model.Username, 
                Password = model.Password, 
                CompanyName = model.CompanyName 
            };
            
            await _authService.RegisterEmployerAsync(dto);

            TempData["Message"] = "ثبت‌نام با موفقیت انجام شد. پس از تایید ادمین می‌توانید وارد سیستم شوید.";
            return RedirectToAction("Login");
        }
        catch (ConflictException)
        {
            ModelState.AddModelError("", "این شماره موبایل قبلاً در سیستم ثبت‌نام کرده است.");
            return View(model);
        }
        catch (UserRegistrationException)
        {
            ModelState.AddModelError("", "خطا در ثبت‌نام: رمز عبور باید حداقل ۶ کاراکتر و شامل حروف و اعداد باشد.");
            return View(model);
        }
        catch (BaseAppException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
