using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.JobAdDto;

public class PublicJobAdSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public Guid? CompanyLogoId { get; set; } 
    public string CityName { get; set; }
    public string ProvinceName { get; set; }
    public string CategoryName { get; set; } 
    public decimal? Salary { get; set; }
    public EmploymentTypeEnum EmploymentType { get; set; }
    public bool IsHighlighted { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
}