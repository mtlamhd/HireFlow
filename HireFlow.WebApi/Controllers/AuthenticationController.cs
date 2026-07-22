using HireFlow.Domain.Dtos.AuthenticationDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }
    [HttpPost("register-jobseeker")]
    public async Task<IActionResult> RegisterJobSeeker([FromBody] RegisterJobSeekerDto dto)
    {
        var result = await _authService.RegisterJobSeekerAsync(dto);
        
        return Ok(GenericResult<RegisterResultDto>.Success(
            result, 
            "Job seeker registered successfully.", 
            201)); 
    }
    
    [HttpPost("register-employer")]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto dto)
    {
        var result = await _authService.RegisterEmployerAsync(dto);
        
        return Ok(GenericResult<RegisterResultDto>.Success(
            result, 
            "Employer registered successfully. Your company has been created and is pending admin approval.", 
            201));
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.TokenLoginAsync(dto);
        
        return Ok(GenericResult<LoginResultDto>.Success(
            result, 
            "Login successful. Welcome to HireFlow.", 
            200));
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);

        return Ok(GenericResult<LoginResultDto>.Success(
            result,
            "Token refreshed successfully.",
            200));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.LogoutAsync(dto);

        return Ok(
            GenericResult<bool>.Success(
                true,
                "Logged out successfully."));
    }
    
}