using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class RegisterEmployerDto
{
    [Required(ErrorMessage = "Username (Phone number) is required.")]
    public string Username { get; set; } 

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } 

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 150 characters.")]
    public string CompanyName { get; set; }
}