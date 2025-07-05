using System.Net.Mail;

namespace Infrastructure.Email;

public class EmailNotificationNotificationSender : IEmailNotificationSender
{
    private readonly SmtpClient _smtpClient;

    public EmailNotificationNotificationSender(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task<bool> SendAsync(string to, string subject, string body)
    {
        try
        {
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            await _smtpClient.SendMailAsync(mail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
       
        return true;
    }
}