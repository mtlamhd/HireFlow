using HireFlow.Domain.Dtos.CategoryDto;

namespace HireFlow.Domain.Dtos.AdminDto;

public class AdminEmployerDetailsDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } 
    public string? FullName { get; set; } 
    public string? Email { get; set; }
    public string? NationalId { get; set; }
    public bool IsApproved { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CompanyId { get; set; }
    public string CompanyName { get; set; } 
    public string? CompanyDescription { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyAddress { get; set; }
    public Guid? CompanyLogoId { get; set; }
    public string? CityName { get; set; }
    public string? ProvinceName { get; set; }
    public int JobAdsCount { get; set; }
    public List<CategoryViewDto> CompanyCategories { get; set; } = new();
}