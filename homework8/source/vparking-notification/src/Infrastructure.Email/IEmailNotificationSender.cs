namespace Infrastructure.Email;

public interface IEmailNotificationSender
{
    Task<bool> SendAsync(string to, string subject, string body);
}