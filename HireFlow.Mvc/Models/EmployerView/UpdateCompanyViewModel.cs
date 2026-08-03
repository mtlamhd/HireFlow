using System.ComponentModel.DataAnnotations;
namespace HireFlow.Mvc.Models.EmployerView
{
    public class UpdateCompanyViewModel
    {
        [Required(ErrorMessage = "وارد کردن نام شرکت الزامی است.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "نام شرکت باید بین ۲ تا ۲۰۰ کاراکتر باشد.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن توضیحات و معرفی شرکت الزامی است.")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "توضیحات معرفی شرکت باید بین ۲۰ تا ۱۰۰۰ کاراکتر باشد.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن آدرس وب‌سایت رسمی شرکت الزامی است.")]
        [StringLength(150, ErrorMessage = "آدرس وب‌سایت نمی‌تواند بیشتر از ۱۵۰ کاراکتر باشد.")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+([\/?#].*)?$", ErrorMessage = "فرمت آدرس وب‌سایت نامعتبر است. مثال: google.com")]
        public string Website { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن ایمیل رسمی شرکت الزامی است.")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل شرکت نامعتبر است.")]
        [StringLength(150, ErrorMessage = "ایمیل نمی‌تواند بیشتر از ۱۵۰ کاراکتر باشد.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن شماره تلفن تماس شرکت الزامی است.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "شماره تلفن باید بین ۵ تا ۲۰ کاراکتر باشد.")]
        [RegularExpression(@"^[0-9+\-\s]+$", ErrorMessage = "شماره تلفن فقط می‌تواند شامل عدد، فاصله، + و - باشد.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن آدرس فیزیکی دفتر شرکت الزامی است.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "آدرس شرکت باید بین ۱۰ تا ۵۰۰ کاراکتر باشد.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "انتخاب شهر محل استقرار شرکت الزامی است.")]
        public Guid? CityId { get; set; }

        public Guid? LogoId { get; set; }

        public List<Guid> CategoryIds { get; set; } = new();
    }
}