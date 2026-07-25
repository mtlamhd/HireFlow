using HireFlow.Domain.Dtos.AdminDto;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IAdminDashboardRepository
{
    Task<AdminDashboardStatsDto> GetDashboardStatsAsync(string jobSeekerRole, string employerRole);
}