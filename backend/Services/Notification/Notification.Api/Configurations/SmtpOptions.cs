namespace Notification.Api.Configurations;

public class SmtpOptions
{
    public const string SectionName = "SmtpOptions";

    public string Host { get; set; } = "sandbox.smtp.mailtrap.io";
    public int Port { get; set; } = 2525;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = "noreply@eproject.local";
    public string SenderName { get; set; } = "eProject Notification Service";
    public bool EnableSsl { get; set; } = true;
    public bool EnableStartTls { get; set; } = true;
}
