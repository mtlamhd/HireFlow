using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.UserDto;

public class UpdateEmployerProfileDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "National ID is required.")]
    public string NationalId { get; set; } = default!;

    [Required(ErrorMessage = "Birth date is required.")]
    public DateTime BirthDate { get; set; }
}