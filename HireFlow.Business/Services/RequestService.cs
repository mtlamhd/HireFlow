using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Business.Services;

public class RequestService : IRequestService
{
    
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public RequestService(IUnitOfWork unitOfWork, UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
        }
        
        public async Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid userId, Guid jobAdId)
        {
           
            var company = await GetApprovedCompanyAsync(userId);

           
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId);
            if (jobAd == null)
                throw new ItemNotFoundException("JobAd", jobAdId);

            if (jobAd.CompanyId != company.Id)
                throw new ResourceAccessDeniedException("JobAd", jobAdId);

            
            return await _unitOfWork.Requests.GetJobAdRequestsAsync(jobAdId);
        }
        
        public async Task<RequestViewDto> GetRequestDetailsAsync(Guid userId, Guid requestId)
        {
            var company = await GetApprovedCompanyAsync(userId);

            
            await VerifyRequestOwnershipAsync(company.Id, requestId);

          
            var details = await _unitOfWork.Requests.GetRequestDetailsAsync(requestId);
            if (details == null)
                throw new ItemNotFoundException("Request", requestId);


            return details;
        }
        
        public async Task ChangeRequestStatusAsync(Guid userId, Guid requestId, ChangeRequestStatusDto dto)
        {
           
            var company = await GetApprovedCompanyAsync(userId);

           
            var request = await VerifyRequestOwnershipAsync(company.Id, requestId, tracking: true);

            
            switch (dto.NewStatus)
            {
                case RequestStatusEnum.UnderReview:
                    request.MoveToUnderReview(userId);
                    break;

                case RequestStatusEnum.Interview:
                    request.MoveToInterview(userId);
                    break;

                case RequestStatusEnum.Accepted:
                    request.Accept(userId);
                    break;

                case RequestStatusEnum.Rejected:
                    request.Reject(userId);
                    break;

                default:
                    throw new InvalidRequestException("Invalid status transition. Employers are not allowed to set this status.");
            }
            await _unitOfWork.SaveChangesAsync();
            var jobSeeker = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (jobSeeker != null && !string.IsNullOrWhiteSpace(jobSeeker.Email))
            {
                var jobAd = await _unitOfWork.JobAds.GetByIdAsync(request.JobAdId);
                if (jobAd != null)
                {
                    
                    var jobSeekerName = (string.IsNullOrWhiteSpace(jobSeeker.FirstName) && string.IsNullOrWhiteSpace(jobSeeker.LastName))
                        ? jobSeeker.UserName!
                        : $"{jobSeeker.FirstName} {jobSeeker.LastName}".Trim();

                   
                    var placeholders = new Dictionary<string, string>
                    {
                        { "{Name}", jobSeekerName },
                        { "{JobTitle}", jobAd.Title },
                        { "{CompanyName}", company.Name }
                    };

                    
                    EmailEventTypeEnum? emailType = dto.NewStatus switch
                    {
                        RequestStatusEnum.UnderReview => EmailEventTypeEnum.RequestUnderReview,
                        RequestStatusEnum.Interview => EmailEventTypeEnum.RequestInterview,
                        RequestStatusEnum.Accepted => EmailEventTypeEnum.RequestAccepted,
                        RequestStatusEnum.Rejected => EmailEventTypeEnum.RequestRejected,
                        _ => null
                    };
                    
                    if (emailType.HasValue)
                    {
                        await _emailService.SendEmailFromTemplateAsync(jobSeeker.Email, emailType.Value, placeholders);
                    }
                }
            }
        }
        
        private async Task<Company> GetApprovedCompanyAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidRequestException("User ID cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ItemNotFoundException("User", userId);

            if (!user.IsApproved)
                throw new EmployerAccountNotApprovedException(userId);

            var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
            if (company == null)
                throw new ItemNotFoundException($"No registered company found for employer with id '{userId}'.");

            return company;
        }
        
        private async Task<Request> VerifyRequestOwnershipAsync(Guid companyId, Guid requestId, bool tracking = false)
        {
            if (requestId == Guid.Empty)
                throw new InvalidRequestException("Request ID cannot be empty.");
            
            
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId, tracking);
            if (request == null)
                throw new ItemNotFoundException("Request", requestId);
            
            
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(request.JobAdId);
            if (jobAd == null || jobAd.CompanyId != companyId)
                throw new ResourceAccessDeniedException("Request", requestId);

            return request;
        }
        
        public async Task ApplyForJobAdAsync(Guid userId, ApplyJobAdDto dto)
        {
            if (userId == Guid.Empty)
                throw new InvalidRequestException("User ID cannot be empty.");

            if (dto == null || dto.JobAdId == Guid.Empty)
                throw new InvalidRequestException("Invalid Job Ad ID.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ItemNotFoundException("User", userId);
            if (!user.IsActive)
            {
                throw new InvalidRequestException("Your account has been deactivated by the admin.");
            }

            if (!user.ResumeId.HasValue || user.ResumeId.Value == Guid.Empty)
            {
                throw new InvalidRequestException("You must upload your resume before applying to any job.");
            }

            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(dto.JobAdId);
            if (jobAd == null)
                throw new ItemNotFoundException("JobAd", dto.JobAdId);

            if (!jobAd.IsActive || jobAd.IsExpired())
            {
                throw new InvalidRequestException("This job advertisement is either inactive or has expired.");
            }

           
            var existingRequest = await _unitOfWork.Requests.GetFirstOrDefaultAsync(
                r => r.UserId == userId && r.JobAdId == dto.JobAdId, 
                tracking: true);

            if (existingRequest != null)
            {
               
                if (existingRequest.Status == RequestStatusEnum.Cancelled)
                {
                    existingRequest.Reapply(userId);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }

                
                throw new ConflictException("شما قبلاً برای این فرصت شغلی درخواست ارسال کرده‌اید.");
            }
            
            var request = new Request(dto.JobAdId, userId);
            
            await _unitOfWork.Requests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
            
            var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.Id == jobAd.CompanyId);
            if (company != null)
            {
                var employer = await _userManager.FindByIdAsync(company.OwnerId.ToString());
                if (employer != null && !string.IsNullOrWhiteSpace(employer.Email))
                {
                    var jobSeekerName = (string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName))
                        ? user.UserName!
                        : $"{user.FirstName} {user.LastName}".Trim();

                    var placeholders = new Dictionary<string, string>
                    {
                        { "{CompanyName}", company.Name },
                        { "{Name}", jobSeekerName },
                        { "{JobTitle}", jobAd.Title }
                    };

                    await _emailService.SendEmailFromTemplateAsync(
                        employer.Email, 
                        EmailEventTypeEnum.NewApplicationReceived, 
                        placeholders);
                }
            }
        }
        
        public async Task<List<JobSeekerRequestSummaryDto>> GetJobSeekerRequestsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidRequestException("User ID cannot be empty.");

           
            var userExists = await _userManager.FindByIdAsync(userId.ToString());
            if (userExists == null)
                throw new ItemNotFoundException("User", userId);

            return await _unitOfWork.Requests.GetJobSeekerRequestsAsync(userId);
        }
        
        public async Task<JobSeekerRequestDetailsDto> GetJobSeekerRequestDetailsAsync(Guid userId, Guid requestId)
        {
            if (userId == Guid.Empty)
                throw new InvalidRequestException("User ID cannot be empty.");

            if (requestId == Guid.Empty)
                throw new InvalidRequestException("Request ID cannot be empty.");

           
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId);
            if (request == null)
                throw new ItemNotFoundException("Request", requestId);
            
            if (request.UserId != userId)
            {
                throw new ResourceAccessDeniedException("Request", requestId);
            }
            
            var detailsDto = await _unitOfWork.Requests.GetJobSeekerRequestDetailsAsync(requestId);
            if (detailsDto == null)
                throw new ItemNotFoundException("Request", requestId);

            return detailsDto;
        }
        public async Task CancelRequestAsync(Guid userId, Guid requestId)
        {
            if (userId == Guid.Empty)
                throw new InvalidRequestException("User ID cannot be empty.");

            if (requestId == Guid.Empty)
                throw new InvalidRequestException("Request ID cannot be empty.");

            
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId, tracking: true);
            if (request == null)
                throw new ItemNotFoundException("Request", requestId);

    
            if (request.UserId != userId)
            {
                throw new ResourceAccessDeniedException("Request", requestId);
            }

            request.Cancel(userId);

           
            await _unitOfWork.SaveChangesAsync();
        }
}
