using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public  interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetActiveTokenWithUserAsync(string token);
}