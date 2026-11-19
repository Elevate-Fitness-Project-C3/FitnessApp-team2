using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace IdentityService.Services.Email;

public sealed class EmailSender(IConfiguration configuration, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(
        string email, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        using var client = new SmtpClient();

        try
        {
            var host = GetRequiredSetting("Host");
            var senderName = GetRequiredSetting("SenderName");
            var senderEmail = GetRequiredSetting("SenderEmail");
            var username = configuration["EmailSettings:Username"]?.Trim();
            var password = GetRequiredSetting("AppPassword");
            var port = configuration.GetValue<int>("EmailSettings:Port");
            if (port is < 1 or > 65535)
                throw new InvalidOperationException("EmailSettings:Port must be between 1 and 65535.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            var security = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            await client.ConnectAsync(host, port, security, timeout.Token);
            await client.AuthenticateAsync(
                string.IsNullOrWhiteSpace(username) ? senderEmail : username, password, timeout.Token);
            await client.SendAsync(new SmtpSendRequest(message), timeout.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning("Email delivery failed ({ErrorType}).", exception.GetType().Name);
            throw new IOException("The email provider could not send the message.");
        }

        try
        {
            await client.DisconnectAsync(quit: true, timeout.Token);
        }
        catch (Exception exception)
        {
            // The SMTP server already accepted the message.
            logger.LogDebug("Email provider disconnect failed ({ErrorType}).", exception.GetType().Name);
        }
    }

    private string GetRequiredSetting(string name)
    {
        var key = $"EmailSettings:{name}";
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Email setting '{key}' is missing.");

        return name == "AppPassword" ? value : value.Trim();
    }
}
