using HireFlow.Business.Authentications.Constants;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.EmailDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdminDashboardRepository _adminDashboardRepository;
    private readonly IEmailService _emailService;

    public AdminService(UserManager<User> userManager, IUserRepository userRepository, IUnitOfWork unitOfWork, IAdminDashboardRepository adminDashboardRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _adminDashboardRepository = adminDashboardRepository;
    }

    public async Task ApproveEmployerAsync(Guid userId, Guid requesterId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            throw new ItemNotFoundException("User", userId);

        user.Approve(requesterId);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Employer Approval", errorMessage);
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
            
            var employerName = (string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName))
                ? user.UserName!
                : $"{user.FirstName} {user.LastName}".Trim();

            var placeholders = new Dictionary<string, string>
            {
                { "{Name}", employerName },
                { "{CompanyName}", company?.Name ?? "ثبت‌نشده" }
            };

            await _emailService.SendEmailFromTemplateAsync(
                user.Email, 
                EmailEventTypeEnum.EmployerApproved, 
                placeholders);
        }
    }

    public async Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync()
    {
        return await _userRepository.GetUnapprovedEmployersAsync();
    }

    public async Task<List<AdminJobSeekerSummaryDto>> GetAllJobSeekersAsync()
    {

        return await _userRepository.GetAllJobSeekersAsync(RoleConstants.JobSeekerRoleName);

    }

    public async Task<AdminJobSeekerDetailsDto> GetJobSeekerDetailsAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Seeker ID cannot be empty.");

        var detailsDto = await _userRepository.GetJobSeekerDetailsForAdminAsync(id);

        if (detailsDto == null)
            throw new ItemNotFoundException("JobSeeker", id);

        return detailsDto;
    }

    public async Task ActivateJobSeekerAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");


        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", id);


        user.Activate(requesterId);


        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Activate JobSeeker", errorMessage);
        }
    }

    public async Task DeactivateJobSeekerAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", id);


        user.Deactivate(requesterId);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Deactivate JobSeeker", errorMessage);
        }

    }
    public async Task<List<AdminJobAdSummaryDto>> GetAllJobAdsForAdminAsync()
    {
        return await _unitOfWork.JobAds.GetAllJobAdsForAdminAsync();
    }
    public async Task<AdminJobAdDetailsDto> GetJobAdDetailsForAdminAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

        var detailsDto = await _unitOfWork.JobAds.GetJobAdDetailsForAdminAsync(id);

        if (detailsDto == null)
            throw new ItemNotFoundException("JobAd", id);

        return detailsDto;
    }
    public async Task ActivateJobAdAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

       
        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", id);

     
        jobAd.Activate(requesterId);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeactivateJobAdAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", id);

        jobAd.Deactivate(requesterId);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task SoftDeleteJobAdAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", id);

       
        _unitOfWork.JobAds.SoftDelete(jobAd, requesterId);

        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task MakeJobAdFeaturedAsync(Guid id, DateTime expiresAt, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

        if (expiresAt <= DateTime.UtcNow)
            throw new InvalidRequestException("Featured expiration date must be in the future.");

        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", id);

       
        jobAd.MakeFeatured(expiresAt, requesterId);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task CancelJobAdFeaturedAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("Job Ad ID cannot be empty.");

        var jobAd = await _unitOfWork.JobAds.GetByIdAsync(id, tracking: true);
        if (jobAd == null)
            throw new ItemNotFoundException("JobAd", id);
        
        jobAd.CancelFeatured(requesterId);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
    {
       
        var stats = await _adminDashboardRepository.GetDashboardStatsAsync(
            RoleConstants.JobSeekerRoleName, 
            RoleConstants.EmployerRoleName);

       
        if (stats == null)
        {
            throw new ItemNotFoundException("Dashboard statistics could not be retrieved from the database.");
        }

        return stats;
    }
    public async Task<List<AdminEmployerSummaryDto>> GetAllEmployersAsync()
    {
       
        return await _userRepository.GetAllEmployersForAdminAsync(RoleConstants.EmployerRoleName);
    }


    public async Task<AdminEmployerDetailsDto> GetEmployerDetailsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var detailsDto = await _userRepository.GetEmployerDetailsForAdminAsync(userId);

        
        if (detailsDto == null)
            throw new ItemNotFoundException("Employer", userId);

        return detailsDto;
    }


    public async Task DisapproveEmployerAsync(Guid userId, Guid requesterId)
    {
        if (userId == Guid.Empty)
            throw new InvalidRequestException("User ID cannot be empty.");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new ItemNotFoundException("User", userId);

        
        user.Disapprove(requesterId);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new IdentityOperationException("Employer Disapproval", errorMessage);
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
            
            var employerName = (string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName))
                ? user.UserName!
                : $"{user.FirstName} {user.LastName}".Trim();

            var placeholders = new Dictionary<string, string>
            {
                { "{Name}", employerName },
                { "{CompanyName}", company?.Name ?? "ثبت‌نشده" }
            };

            await _emailService.SendEmailFromTemplateAsync(
                user.Email, 
                EmailEventTypeEnum.EmployerDisapproved, 
                placeholders);
        }
    }
     public async Task<List<EmailTemplateDto>> GetAllEmailTemplatesAsync()
    {
        return await _unitOfWork.EmailTemplates.GetAllTemplatesAsync();
    }

    public async Task<EmailTemplateDto> GetEmailTemplateByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("شناسه قالب ایمیل نمی‌تواند خالی باشد.");

        var dto = await _unitOfWork.EmailTemplates.GetTemplateByIdAsync(id);
        
        if (dto == null)
            throw new ItemNotFoundException("EmailTemplate", id);

        return dto;
    }

    public async Task UpdateEmailTemplateAsync(Guid id, UpdateEmailTemplateDto dto, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("شناسه قالب ایمیل نمی‌تواند خالی باشد.");

    
        var template = await _unitOfWork.EmailTemplates.GetTemplateEntityByIdAsync(id);
        
        if (template == null)
            throw new ItemNotFoundException("EmailTemplate", id);

     
        template.UpdateTemplate(dto.Subject, dto.Body, requesterId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ActivateEmailTemplateAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("شناسه قالب ایمیل نمی‌تواند خالی باشد.");

        var template = await _unitOfWork.EmailTemplates.GetTemplateEntityByIdAsync(id);
        
        if (template == null)
            throw new ItemNotFoundException("EmailTemplate", id);

        template.Activate(requesterId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateEmailTemplateAsync(Guid id, Guid requesterId)
    {
        if (id == Guid.Empty)
            throw new InvalidRequestException("شناسه قالب ایمیل نمی‌تواند خالی باشد.");

        var template = await _unitOfWork.EmailTemplates.GetTemplateEntityByIdAsync(id);
        
        if (template == null)
            throw new ItemNotFoundException("EmailTemplate", id);

        template.Deactivate(requesterId);

        await _unitOfWork.SaveChangesAsync();
    }

    }

