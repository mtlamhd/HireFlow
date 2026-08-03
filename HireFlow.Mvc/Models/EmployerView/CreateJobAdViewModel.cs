using System.ComponentModel.DataAnnotations;
using HireFlow.Domain.Enums;

namespace HireFlow.Mvc.Models.EmployerView;

public class CreateJobAdViewModel
{
    [Required(ErrorMessage = "وارد کردن عنوان شغلی الزامی است.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "عنوان شغلی باید بین ۳ تا ۲۰۰ کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن شرح موقعیت شغلی الزامی است.")]
    [MinLength(10, ErrorMessage = "شرح موقعیت شغلی باید حداقل ۱۰ کاراکتر باشد.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب شهر محل کار الزامی است.")]
    public Guid? CityId { get; set; } // 👈 اینجا علامت سوال اضافه کن

    [Required(ErrorMessage = "انتخاب دسته‌بندی شغلی الزامی است.")]
    public Guid? CategoryId { get; set; } // 👈 اینجا علامت سوال اضافه کن

    [Range(0, double.MaxValue, ErrorMessage = "حقوق و دستمزد باید یک عدد مثبت باشد.")]
    public decimal? Salary { get; set; }

    [Required(ErrorMessage = "انتخاب نوع همکاری الزامی است.")]
    public EmploymentTypeEnum? EmploymentType { get; set; } // 👈 اینجا علامت سوال اضافه کن

    public List<Guid> SkillIds { get; set; } = new();
}
