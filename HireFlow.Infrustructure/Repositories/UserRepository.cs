using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;


    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context; // The Identity DbContext

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsApproved)
                .AsNoTracking()
                .Select(u => new PendingEmployerDto
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    CompanyName = u.Companies.Select(c => c.Name).FirstOrDefault()!
                    
                })
                .ToListAsync();
        }
    }