using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.JobAdDto;

public class JobAdDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string Description { get; set; } 
    public Guid CityId { get; set; }
    public string CityName { get; set; } 
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public decimal? Salary { get; set; }
    public EmploymentTypeEnum EmploymentType { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpireAt { get; set; }
    public List<CategoryViewDto> Skills { get; set; } = new();
}