using HireFlow.Domain.Dtos.AuthenticationDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
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
        return Ok(result);
    }
    [HttpPost("register-employer")]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto dto)
    {
        var result = await _authService.RegisterEmployerAsync(dto);
        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.TokenLoginAsync(dto);
        return Ok(result);
    }
}