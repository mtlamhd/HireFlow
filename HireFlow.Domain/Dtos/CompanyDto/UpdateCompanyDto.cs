using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.CompanyDto;

public class UpdateCompanyDto
{
    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 200 characters.")]
    public string Name { get; set; }

    [StringLength(1000, ErrorMessage = "Company description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid website URL format.")]
    public string? Website { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid company phone number format.")]
    public string? PhoneNumber { get; set; }

    [StringLength(500, ErrorMessage = "Company address cannot exceed 500 characters.")]
    public string? Address { get; set; }
}