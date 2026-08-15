namespace HireFlow.Infrustructure.EmailServices;

public class EmailSettings
{
    public string Host { get; set; } 
    public int Port { get; set; }
    public string Username { get; set; } 
    public string Password { get; set; }
    public string SenderEmail { get; set; } 
    public string SenderName { get; set; } 
    public bool IsHtml { get; set; } = false;
    public bool UseSsl { get; set; } = false; 
}