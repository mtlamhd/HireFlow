using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.RequestDto;

public class RequestSummaryDto
{
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string JobSeekerName { get; set; } 
    public string JobSeekerPhoneNumber { get; set; } 
    public RequestStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}