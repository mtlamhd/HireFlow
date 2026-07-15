using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;

namespace HireFlow.Infrustructure.Repositories;

public class SkillRepository : GenericRepository<Skill> , ISkillRepository
{
    public SkillRepository(AppDbContext context) : base(context)
    {
    }
}