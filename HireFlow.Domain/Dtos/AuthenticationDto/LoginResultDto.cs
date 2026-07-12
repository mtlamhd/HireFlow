namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class LoginResultDto
{
    public string AccessToken { get; set; }
    public double ExpiresIn { get; set; }

    public LoginResultDto(string accessToken, double expiresIn)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
    }
}