namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class LoginResultDto
{
    public string AccessToken { get; set; } 
    public double ExpiresInSeconds { get; set; }
    public string RefreshToken { get; set; } 

    public LoginResultDto(string accessToken, double expiresInSeconds, string refreshToken)
    {
        AccessToken = accessToken;
        ExpiresInSeconds = expiresInSeconds;
        RefreshToken = refreshToken;
    }
}