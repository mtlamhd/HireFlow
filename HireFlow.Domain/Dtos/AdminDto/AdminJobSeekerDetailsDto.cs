using HireFlow.Domain.Dtos.SkillDto;

namespace HireFlow.Domain.Dtos.AdminDto;

public class AdminJobSeekerDetailsDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } 
    public string FullName { get; set; } 
    public string? Email { get; set; }
    public string? NationalId { get; set; }
    public DateTime? BirthDate { get; set; }
    
   
    public int? Age 
    { 
        get 
        {
            if (!BirthDate.HasValue)
                return null;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - BirthDate.Value.Year;

            if (BirthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ResumeId { get; set; }
    public string? ResumeFileName { get; set; }
    public List<SkillViewDto> Skills { get; set; } = new();
}