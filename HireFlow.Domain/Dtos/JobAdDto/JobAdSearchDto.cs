using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.JobAdDto;

public class JobAdSearchDto : Paging
{
  
    public string? Title { get; set; }
    public EmploymentTypeEnum? EmploymentType { get; set; }
    public Guid? CityId { get; set; }
    
    public Guid? CategoryId { get; set; }
    public decimal? MinSalary { get; set; }
    public List<Guid> SkillIds { get; set; } = new();
}