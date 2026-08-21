using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Request = HireFlow.Domain.Entities.Request;

namespace HireFlow.Infrastructure.Repositories;

public class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<List<RequestSummaryDto>> GetJobAdRequestsAsync(Guid jobAdId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.JobAdId == jobAdId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RequestSummaryDto
            {
                Id = r.Id,
                UserId = r.UserId,
               
                JobSeekerName = (string.IsNullOrWhiteSpace(r.User.FirstName) && string.IsNullOrWhiteSpace(r.User.LastName))
                    ? r.User.UserName!
                    : (r.User.FirstName + " " + r.User.LastName).Trim(),
                JobSeekerPhoneNumber = r.User.PhoneNumber!,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }
    public async Task<RequestViewDto?> GetRequestDetailsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RequestViewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                FirstName = r.User.FirstName ?? string.Empty,
                LastName = r.User.LastName ?? string.Empty,
                Email = r.User.Email,
                PhoneNumber = r.User.PhoneNumber!,
                NationalId = r.User.NationalId,
               
                Age = r.User.BirthDate.HasValue 
                    ? DateTime.UtcNow.Year - r.User.BirthDate.Value.Year 
                    : null,
                
               
                ResumeId = r.User.ResumeId ?? Guid.Empty,
                ResumeFileName = r.User.Resume != null ? r.User.Resume.FileName : string.Empty,
                
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<JobSeekerRequestSummaryDto>> GetJobSeekerRequestsAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt) 
            .Select(r => new JobSeekerRequestSummaryDto
            {
                Id = r.Id,
                JobAdId = r.JobAdId,
                JobAdTitle = r.JobAd.Title,
                CompanyName = r.JobAd.Company.Name,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }
    
    public async Task<JobSeekerRequestDetailsDto?> GetJobSeekerRequestDetailsAsync(Guid requestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.Id == requestId)
            .Select(r => new JobSeekerRequestDetailsDto
            {
                Id = r.Id,
                JobAdId = r.JobAdId,
                JobAdTitle = r.JobAd.Title,
                CompanyName = r.JobAd.Company.Name,
                JobAdDescription = r.JobAd.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> HasAlreadyAppliedAsync(Guid userId, Guid jobAdId)
    {
        return await _dbSet.AnyAsync(r => 
            r.UserId == userId && 
            r.JobAdId == jobAdId && 
            r.Status != RequestStatusEnum.Cancelled);
    }
    
}
