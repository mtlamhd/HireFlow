using Azure.Core;
using HireFlow.Domain.Dtos.RequestDto;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;
using Request = HireFlow.Domain.Entities.Request;

namespace HireFlow.Infrustructure.Repositories;

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
    
}
