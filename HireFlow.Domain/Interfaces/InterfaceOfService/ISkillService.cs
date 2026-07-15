using HireFlow.Domain.Dtos.SkillDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface ISkillService
{
    Task<List<SkillViewDto>> GetAllSkillsAsync();
}