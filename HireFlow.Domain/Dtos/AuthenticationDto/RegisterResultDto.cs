namespace HireFlow.Domain.Dtos.AuthenticationDto;

public class RegisterResultDto
{
    public Guid Id { get; set; }

    public RegisterResultDto(Guid id)
    {
        Id = id;
    }
}