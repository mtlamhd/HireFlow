using HireFlow.Domain.Dtos.CategoryDto;
using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.AdminDto;

public class AdminJobAdDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string Description { get; set; } 
    public decimal? Salary { get; set; }
    public EmploymentTypeEnum EmploymentType { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
   
    public DateTime? HighlightExpireAt { get; set; }
    public bool IsHighlighted { get; set; }

    
    public bool IsFeatured { get; set; }
    public DateTime? FeaturedUntil { get; set; }
    public bool IsFeaturedActive { get; set; }

   
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } 
    public Guid? CompanyLogoId { get; set; }

  
    public string CityName { get; set; } 
    public string ProvinceName { get; set; } 
    public string CategoryName { get; set; } 

    
    public List<SkillViewDto> Skills { get; set; } = new();
}