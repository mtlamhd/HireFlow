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
        
}