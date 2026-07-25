using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly AppDbContext _context;

    public AdminDashboardRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync(string jobSeekerRole, string employerRole)
    {
      
        var jobSeekerRoleId = await _context.Roles
            .Where(r => r.Name == jobSeekerRole)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        var employerRoleId = await _context.Roles
            .Where(r => r.Name == employerRole)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        var stats = new AdminDashboardStatsDto
        {
           
            TotalJobSeekers = await _context.Users
                .CountAsync(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == jobSeekerRoleId)),
            
            TotalEmployers = await _context.Users
                .CountAsync(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == employerRoleId)),
            
            TotalPendingEmployers = await _context.Users
                .CountAsync(u => !u.IsApproved && 
                                  _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == employerRoleId)),

            
            TotalActiveJobAds = await _context.JobAds
                .CountAsync(j => j.IsActive && j.ExpireAt > DateTime.UtcNow),
            
            TotalInactiveJobAds = await _context.JobAds
                .CountAsync(j => !j.IsActive || j.ExpireAt <= DateTime.UtcNow),

           
            InitialRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.Initial),
            
            UnderReviewRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.UnderReview),
            
            InterviewRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.Interview),
            
            AcceptedRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.Accepted),
            
            RejectedRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.Rejected),
            
            CancelledRequestsCount = await _context.Requests
                .CountAsync(r => r.Status == RequestStatusEnum.Cancelled)
        };

        return stats;
    }
}
