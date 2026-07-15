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

        public RequestService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        
        public async Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid userId, Guid jobAdId)
        {
           
            var company = await GetApprovedCompanyAsync(userId);

           
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(jobAdId);
            if (jobAd == null)
                throw new Exception("Job ad not found.");

            if (jobAd.CompanyId != company.Id)
                throw new Exception("Access Denied. You do not own this job ad.");

            
            return await _unitOfWork.Requests.GetJobAdRequestsAsync(jobAdId);
        }
        
        public async Task<RequestViewDto> GetRequestDetailsAsync(Guid userId, Guid requestId)
        {
            // ۱. تایید صلاحیت کارفرما و دریافت شرکت او
            var company = await GetApprovedCompanyAsync(userId);

            
            await VerifyRequestOwnershipAsync(company.Id, requestId);

          
            var details = await _unitOfWork.Requests.GetRequestDetailsAsync(requestId);
            if (details == null)
                throw new Exception("Request details not found.");

            return details;
        }
        
        public async Task ChangeRequestStatusAsync(Guid userId, Guid requestId, ChangeRequestStatusDto dto)
        {
            // ۱. تایید صلاحیت کارفرما و دریافت شرکت او
            var company = await GetApprovedCompanyAsync(userId);

            // ۲. بررسی مالکیت درخواست و واکشی انتیتی با قابلیت Tracking برای اعمال تغییرات
            var request = await VerifyRequestOwnershipAsync(company.Id, requestId, tracking: true);

            // ۳. اعمال تغییر وضعیت بر اساس چرخه‌ی وضعیت دامین (Rich Domain Logic)
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
                   
                    throw new Exception("Invalid status transition. Employers are not allowed to set this status.");
            }
            await _unitOfWork.SaveChangesAsync();
        }
        
        private async Task<Company> GetApprovedCompanyAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new Exception("User ID cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found.");

            if (!user.IsApproved)
                throw new Exception("Your employer account is not approved by system admin yet.");

            var company = await _unitOfWork.Companies.GetFirstOrDefaultAsync(c => c.OwnerId == userId);
            if (company == null)
                throw new Exception("No registered company found for this employer.");

            return company;
        }
        
        private async Task<Request> VerifyRequestOwnershipAsync(Guid companyId, Guid requestId, bool tracking = false)
        {
            if (requestId == Guid.Empty)
                throw new Exception("Invalid request ID.");
            
            
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId, tracking);
            if (request == null)
                throw new Exception("The requested job application does not exist.");
            
            
            var jobAd = await _unitOfWork.JobAds.GetByIdAsync(request.JobAdId);
            if (jobAd == null || jobAd.CompanyId != companyId)
                throw new Exception("Access Denied. You do not have permission to view or manage this application.");

            return request;
        }
        
}