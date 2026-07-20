using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.UserDto;

public class UpdateJobSeekerProfileDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } 
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public string NationalId { get; set; }
    public List<Guid> SkillIds { get; set; } = new();
}