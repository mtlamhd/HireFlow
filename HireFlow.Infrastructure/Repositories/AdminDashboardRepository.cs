using System.Data;
using Dapper;
using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.Data.SqlClient;
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
            .AsNoTracking()
            .Where(r => r.Name == jobSeekerRole)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        var employerRoleId = await _context.Roles
            .AsNoTracking()
            .Where(r => r.Name == employerRole)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

      
        var totalJobSeekers = await _context.Users
            .CountAsync(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == jobSeekerRoleId));

        var totalEmployers = await _context.Users
            .CountAsync(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == employerRoleId));

        var totalPendingEmployers = await _context.Users
            .CountAsync(u => !u.IsApproved && _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == employerRoleId));

        var totalActiveJobAds = await _context.JobAds
            .CountAsync(j => j.IsActive && j.ExpireAt > DateTime.UtcNow);

        var totalInactiveJobAds = await _context.JobAds
            .CountAsync(j => !j.IsActive || j.ExpireAt <= DateTime.UtcNow);

        
        var initialRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.Initial);
        var underReviewRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.UnderReview);
        var interviewRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.Interview);
        var acceptedRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.Accepted);
        var rejectedRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.Rejected);
        var cancelledRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatusEnum.Cancelled);

      
        return new AdminDashboardStatsDto
        {
            TotalJobSeekers = totalJobSeekers,
            TotalEmployers = totalEmployers,
            TotalPendingEmployers = totalPendingEmployers,
            TotalActiveJobAds = totalActiveJobAds,
            TotalInactiveJobAds = totalInactiveJobAds,
            InitialRequestsCount = initialRequests,
            UnderReviewRequestsCount = underReviewRequests,
            InterviewRequestsCount = interviewRequests,
            AcceptedRequestsCount = acceptedRequests,
            RejectedRequestsCount = rejectedRequests,
            CancelledRequestsCount = cancelledRequests
        };
    }
}
