using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Infrastructure.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HireFlow.Infrastructure.EmailServices;

public class EmailService : IEmailService
{
    private readonly AppDbContext _context;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        AppDbContext context,
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _context = context;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailFromTemplateAsync(
        string toEmail, 
        EmailEventTypeEnum eventType, 
        Dictionary<string, string> placeholders)
    {
        var template = await _context.EmailTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Type == eventType && t.IsActive && !t.IsDeleted);

        
        if (template == null)
        {
            _logger.LogWarning("Email template of type {EventType} is inactive or missing.", eventType);
            return;
        }

        
        var subject = template.Subject;
        var body = template.Body;

        if (placeholders != null)
        {
            foreach (var placeholder in placeholders)
            {
                subject = subject.Replace(placeholder.Key, placeholder.Value);
                body = body.Replace(placeholder.Key, placeholder.Value);
            }
        }

        
        _ = Task.Run(async () =>
        {
            try
            {
                await SendRawEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send background email to {ToEmail}", toEmail);
            }
        });
    }

    public async Task SendRawEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress(string.Empty, toEmail));
        message.Subject = subject;

        message.Body = new TextPart(_emailSettings.IsHtml ? "html" : "plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        
        var secureSocketOptions = _emailSettings.UseSsl 
            ? SecureSocketOptions.SslOnConnect 
            : SecureSocketOptions.StartTlsWhenAvailable;

        await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, secureSocketOptions);

        if (!string.IsNullOrWhiteSpace(_emailSettings.Username))
        {
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
        
        _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
    }
}
