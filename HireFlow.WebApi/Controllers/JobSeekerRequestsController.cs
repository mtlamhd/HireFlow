using System.Security.Claims;
using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.WebApi.ResultPaterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.JobSeekerRoleName)]
    public class JobSeekerRequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public JobSeekerRequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }
        
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForJobAd([FromBody] ApplyJobAdDto dto)
        {
            var userId = GetCurrentUserId();
            await _requestService.ApplyForJobAdAsync(userId, dto);

            return Ok(GenericResult<bool>.Success(true, "Your application has been submitted successfully."));
        }

        
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetCurrentUserId();
            var requests = await _requestService.GetJobSeekerRequestsAsync(userId);

            return Ok(GenericResult<List<JobSeekerRequestSummaryDto>>.Success(requests));
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestDetails(Guid id)
        {
            var userId = GetCurrentUserId();
            var requestDetails = await _requestService.GetJobSeekerRequestDetailsAsync(userId, id);

            return Ok(GenericResult<JobSeekerRequestDetailsDto>.Success(requestDetails));
        }

       
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelRequest(Guid id)
        {
            var userId = GetCurrentUserId();
            await _requestService.CancelRequestAsync(userId, id);

            return Ok(GenericResult<Guid>.Success(id, "Your application has been cancelled successfully."));
        }
        
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(userIdClaim);
        }
    }