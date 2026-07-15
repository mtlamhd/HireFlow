using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.RequestDto;

public class RequestViewDto
{
    public Guid Id { get; set; } 
    public Guid UserId { get; set; } 
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } 
    public string? NationalId { get; set; }
    public int? Age { get; set; }
    
  
    public Guid ResumeId { get; set; } 
    public string ResumeFileName { get; set; } 

    public RequestStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
