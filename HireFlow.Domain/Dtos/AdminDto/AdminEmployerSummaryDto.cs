namespace HireFlow.Domain.Dtos.AdminDto;

public class AdminEmployerSummaryDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } 
    public string FullName { get; set; } 
    public string CompanyName { get; set; }
    public bool IsApproved { get; set; } 
    public bool IsActive { get; set; } 
    public DateTime CreatedAt { get; set; }
}