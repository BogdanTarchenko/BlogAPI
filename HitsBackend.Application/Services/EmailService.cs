using MailKit.Net.Smtp;
using MimeKit;

namespace HitsBackend.Application.Services;

public class EmailService
{
    private readonly string _smtpServer = "localhost";
    private readonly int _smtpPort = 1025;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Blog Notifications", "no-reply@blog.com"));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        int maxRetries = 3;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.None);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return;
            }
            catch (Exception ex)
            {
                attempt++;
                
                if (attempt >= maxRetries)
                {
                    throw new Exception($"Failed to send email to {to}", ex);
                }
            }
        }
    }
}