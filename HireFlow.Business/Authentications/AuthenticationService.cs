using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.AuthenticationDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HireFlow.Business.Authentications;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _jwtSettings = options.Value;
    }

    public async Task<RegisterResultDto> RegisterJobSeekerAsync(RegisterJobSeekerDto dto)
    {
        var duplicateUser = await _userManager.FindByNameAsync(dto.Username);
        if (duplicateUser != null)
            throw new Exception("User already exists.");

        var user = new User(dto.Username, isApproved: true);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception("user creation failed.");

        await _userManager.AddToRoleAsync(user, RoleConstants.JobSeekerRoleName);

        return new RegisterResultDto(user.Id);
    }

    public async Task<RegisterResultDto> RegisterEmployerAsync(RegisterEmployerDto dto)
    {
        var duplicateUser = await _userManager.FindByNameAsync(dto.Username);
        if (duplicateUser != null)
            throw new Exception("User already exists.");

        var user = new User(dto.Username, isApproved: false);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);

        await _userManager.AddToRoleAsync(user, RoleConstants.EmployerRoleName);

        var company = new Company(dto.CompanyName, user.Id);
        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return new RegisterResultDto(user.Id);
    }

    public async Task<LoginResultDto> TokenLoginAsync(LoginDto dto)
    {

        var result = await _signInManager.PasswordSignInAsync(
            dto.Username, dto.Password, false, true);

        if (result.IsLockedOut)
            throw new Exception("User is locked out. Please try again later.");

        if (!result.Succeeded)
            throw new Exception("Invalid username or password.");

        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
            throw new Exception("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(RoleConstants.EmployerRoleName) && !user.IsApproved)
            throw new Exception("Employer account is not approved yet.");

        return await GenerateTokenAsync(user);
    }

    
        private async Task<LoginResultDto> GenerateTokenAsync(User user)
        {
            var displayName = string.IsNullOrWhiteSpace(user.FirstName)
                ? user.PhoneNumber!
                : $"{user.FirstName} {user.LastName}";

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, displayName),
                new("IsApproved", user.IsApproved.ToString().ToLower())
            };

            var userRoles = (await _userManager.GetRolesAsync(user))
                .Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            foreach (var claim in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(claim.Value);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }
            }

            claims.AddRange(userRoles);

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expiresIn,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var expiresInSeconds = expiresIn.Subtract(DateTime.UtcNow).TotalSeconds;

            return new LoginResultDto(accessToken, expiresInSeconds);
        }
    }


