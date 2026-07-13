using HireFlow.Domain.Dtos.AuthenticationDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IAuthenticationService
{
    Task<RegisterResultDto> RegisterJobSeekerAsync(RegisterJobSeekerDto dto);
    
    Task<RegisterResultDto> RegisterEmployerAsync(RegisterEmployerDto dto);
    
    Task<LoginResultDto> TokenLoginAsync(LoginDto dto);  
}