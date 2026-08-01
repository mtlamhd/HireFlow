using System.ComponentModel.DataAnnotations;

namespace HireFlow.Mvc.Models.AuthView;

public class RegisterJobSeekerViewModel
{
    
        [Required(ErrorMessage = "وارد کردن شماره موبایل الزامی است.")]
        [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "شماره موبایل باید ۱۱ رقم باشد، با 09 شروع شود و فقط شامل عدد باشد.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است.")]
        [MinLength(6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد.")]
        public string Password { get; set; } = default!;
    
}