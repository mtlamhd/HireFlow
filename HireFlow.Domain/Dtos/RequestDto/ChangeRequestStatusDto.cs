using System.ComponentModel.DataAnnotations;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.RequestDto;

public class ChangeRequestStatusDto
{
    [Required(ErrorMessage = "Target status is required.")]
    public RequestStatusEnum NewStatus { get; set; }
}