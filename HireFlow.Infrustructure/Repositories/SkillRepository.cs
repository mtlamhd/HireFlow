using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class SkillRepository : GenericRepository<Skill> , ISkillRepository
{
    public SkillRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<List<SkillViewDto>> GetAllSkillsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SkillViewDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
    }
}