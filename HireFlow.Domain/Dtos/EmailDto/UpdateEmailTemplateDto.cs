using System.ComponentModel.DataAnnotations;

namespace HireFlow.Domain.Dtos.EmailDto;

public class UpdateEmailTemplateDto
{
    
    [Required(ErrorMessage = "موضوع قالب ایمیل الزامی است.")]
    [StringLength(200, ErrorMessage = "موضوع قالب ایمیل نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "بدنه قالب ایمیل الزامی است.")]
    public string Body { get; set; }
}