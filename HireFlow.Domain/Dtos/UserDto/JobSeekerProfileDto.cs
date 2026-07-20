using HireFlow.Domain.Dtos.SkillDto;

namespace HireFlow.Domain.Dtos.UserDto;

public class JobSeekerProfileDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
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
    public string? NationalId { get; set; }
    public Guid? ProfileImageId { get; set; }
    public Guid? ResumeId { get; set; } 
    public List<SkillViewDto> Skills { get; set; } = new();
}