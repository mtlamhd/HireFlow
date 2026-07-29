using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.EmailDto;

public class EmailTemplateDto
{
    public Guid Id { get; set; }
    public EmailEventTypeEnum Type { get; set; }
    
   
    public string TypeName => Type switch
    {
        EmailEventTypeEnum.EmployerApproved => "تایید کارفرما",
        EmailEventTypeEnum.EmployerDisapproved => "رد صلاحیت کارفرما",
        EmailEventTypeEnum.RequestUnderReview => "تغییر وضعیت درخواست به در حال بررسی",
        EmailEventTypeEnum.RequestInterview => "دعوت کارجو به مصاحبه",
        EmailEventTypeEnum.RequestAccepted => "پذیرش نهایی کارجو",
        EmailEventTypeEnum.RequestRejected => "رد درخواست کارجو",
        EmailEventTypeEnum.NewApplicationReceived => "دریافت درخواست همکاری جدید برای کارفرما",
        _ => Type.ToString()
    };

    public string Subject { get; set; } 
    public string Body { get; set; } 
    public bool IsActive { get; set; }
}