using System.ComponentModel.DataAnnotations;
using HireFlow.Domain.Dtos.SkillDto;

namespace HireFlow.Mvc.Models.JobSeekerView;

public class JobSeekerProfileViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "وارد کردن نام الزامی است.")]
    [MinLength(2, ErrorMessage = "نام باید حداقل ۲ حرف باشد.")]
    [StringLength(100, ErrorMessage = "نام نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است.")]
    [MinLength(2, ErrorMessage = "نام خانوادگی باید حداقل ۲ حرف باشد.")]
    [StringLength(100, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string? LastName { get; set; }

    public string PhoneNumber { get; set; } 

    [Required(ErrorMessage = "وارد کردن ایمیل الزامی است.")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "وارد کردن تاریخ تولد الزامی است.")]
    public DateTime? BirthDate { get; set; }

    public int? Age { get; set; }

    [Required(ErrorMessage = "وارد کردن کد ملی الزامی است.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید دقیقاً ۱۰ رقم باشد.")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "کد ملی فقط باید شامل اعداد باشد.")]
    public string? NationalId { get; set; }

    public Guid? ProfileImageId { get; set; }
    public Guid? ResumeId { get; set; } 

    public List<SkillViewDto> Skills { get; set; } = new();
    public List<Guid> SkillIds { get; set; } = new();
}