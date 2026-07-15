using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class SkillService : ISkillService
{
    private  readonly IUnitOfWork _unitOfWork;

    public SkillService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SkillViewDto>> GetAllSkillsAsync()
    {
        return await _unitOfWork.Skills.GetAllSkillsAsync();
    }
}