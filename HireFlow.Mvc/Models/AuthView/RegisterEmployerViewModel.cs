using System.ComponentModel.DataAnnotations;

namespace HireFlow.Mvc.Models.AuthView;

public class RegisterEmployerViewModel
{
    
        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        public string Username { get; set; } 

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        [Required(ErrorMessage = "نام شرکت الزامی است.")]
        public string CompanyName { get; set; } 
    
}