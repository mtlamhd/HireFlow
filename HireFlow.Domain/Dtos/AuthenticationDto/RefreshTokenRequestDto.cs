using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } 
}