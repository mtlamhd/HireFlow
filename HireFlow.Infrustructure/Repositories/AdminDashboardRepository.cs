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
        
        var connectionString = _context.Database.GetDbConnection().ConnectionString;

       
        const string sql = @"
            SELECT 
               
                (SELECT COUNT(*) FROM AspNetUsers u 
                 WHERE EXISTS (
                     SELECT 1 FROM AspNetUserRoles ur 
                     INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
                     WHERE ur.UserId = u.Id AND r.Name = @JobSeekerRole
                 ) AND u.IsDeleted = 0) AS TotalJobSeekers,

                
                (SELECT COUNT(*) FROM AspNetUsers u 
                 WHERE EXISTS (
                     SELECT 1 FROM AspNetUserRoles ur 
                     INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
                     WHERE ur.UserId = u.Id AND r.Name = @EmployerRole
                 ) AND u.IsDeleted = 0) AS TotalEmployers,

             
                (SELECT COUNT(*) FROM AspNetUsers u 
                 WHERE EXISTS (
                     SELECT 1 FROM AspNetUserRoles ur 
                     INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
                     WHERE ur.UserId = u.Id AND r.Name = @EmployerRole
                 ) AND u.IsApproved = 0 AND u.IsDeleted = 0) AS TotalPendingEmployers,

             
                (SELECT COUNT(*) FROM JobAds 
                 WHERE IsActive = 1 AND ExpireAt > GETUTCDATE() AND IsDeleted = 0) AS TotalActiveJobAds,

               
                (SELECT COUNT(*) FROM JobAds 
                 WHERE (IsActive = 0 OR ExpireAt <= GETUTCDATE()) AND IsDeleted = 0) AS TotalInactiveJobAds,

             
                (SELECT COUNT(*) FROM Requests WHERE Status = 0 AND IsDeleted = 0) AS InitialRequestsCount,
                (SELECT COUNT(*) FROM Requests WHERE Status = 1 AND IsDeleted = 0) AS UnderReviewRequestsCount,
                (SELECT COUNT(*) FROM Requests WHERE Status = 2 AND IsDeleted = 0) AS InterviewRequestsCount,
                (SELECT COUNT(*) FROM Requests WHERE Status = 3 AND IsDeleted = 0) AS AcceptedRequestsCount,
                (SELECT COUNT(*) FROM Requests WHERE Status = 4 AND IsDeleted = 0) AS RejectedRequestsCount,
                (SELECT COUNT(*) FROM Requests WHERE Status = 5 AND IsDeleted = 0) AS CancelledRequestsCount
        ";

       
        using var connection = new SqlConnection(connectionString);

        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

       
        var stats = await connection.QuerySingleAsync<AdminDashboardStatsDto>(sql, new
        {
            JobSeekerRole = jobSeekerRole,
            EmployerRole = employerRole
        });

        return stats;
    }
}

