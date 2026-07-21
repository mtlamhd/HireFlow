using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.RequestDto;

public class JobSeekerRequestDetailsDto
{
    public Guid Id { get; set; }
    public Guid JobAdId { get; set; }
    public string JobAdTitle { get; set; } 
    public string CompanyName { get; set; } 
    public string JobAdDescription { get; set; } 
    public RequestStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}