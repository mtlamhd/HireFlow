using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Entities;
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
    public async Task CreateSkillAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRequestException("Skill name cannot be empty.");

        var trimmedName = name.Trim();

       
        var exists = await _unitOfWork.Skills.AnyAsync(s => s.Name.ToLower() == trimmedName.ToLower());
        if (exists)
            throw new ConflictException($"Skill with name '{trimmedName}' already exists.");

        var skill = new Skill(trimmedName);
        await _unitOfWork.Skills.AddAsync(skill);
        await _unitOfWork.SaveChangesAsync();
    }
}