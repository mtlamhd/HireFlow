using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class LoginDto
{
    [Required(ErrorMessage = "Username (Phone number) is required.")]
    public string Username { get; set; } 
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } 
}