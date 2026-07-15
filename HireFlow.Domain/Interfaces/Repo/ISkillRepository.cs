using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface ISkillRepository : IGenericRepository<Skill>
{
    Task<List<SkillViewDto>> GetAllSkillsAsync();
}