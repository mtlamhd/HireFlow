using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IEmailService
{
    Task SendEmailFromTemplateAsync(string toEmail, EmailEventTypeEnum eventType, Dictionary<string, string> placeholders);

   
    Task SendRawEmailAsync(string toEmail, string subject, string body);
}