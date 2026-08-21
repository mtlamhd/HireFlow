using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetActiveTokenWithUserAsync(string token)
    {
        return await _dbSet
            .Include(t => t.User) 
            .FirstOrDefaultAsync(t => t.Token == token);
    }
}