using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.RequestDto;

public class ApplyJobAdDto
{
    [Required(ErrorMessage = "Job Ad ID is required.")]
    public Guid JobAdId { get; set; }
    
}