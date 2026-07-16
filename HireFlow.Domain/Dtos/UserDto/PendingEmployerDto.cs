namespace HireFlow.Domain.Dtos.UserDto;

public class PendingEmployerDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string CompanyName { get; set; }
    public Guid CompanyId { get; set; }
}