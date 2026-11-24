using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace IdentityService.Services.Email;

public sealed class EmailSender(IConfiguration configuration, IHostEnvironment environment) : IEmailSender
{
    private readonly IConfigurationSection settings = configuration.GetRequiredSection("EmailSettings");

    public async Task SendEmailAsync(
        string email, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        var senderEmail = GetRequiredSetting("SenderEmail");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(GetRequiredSetting("SenderName"), senderEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlMessage };

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        var token = timeout.Token;
        using var client = new SmtpClient();

        await ConnectAsync(client, senderEmail, token);
        await client.SendAsync(new SmtpSendRequest(message), token);
        await client.DisconnectAsync(quit: true, token);
    }

    private async Task ConnectAsync(SmtpClient client, string senderEmail, CancellationToken token)
    {
        var host = GetRequiredSetting("Host");
        var port = settings.GetValue<int>("Port");
        if (port is < 1 or > 65535)
            throw new InvalidOperationException("EmailSettings:Port must be between 1 and 65535.");

        if (environment.IsDevelopment() && settings.GetValue<bool>("LocalInbox"))
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.None, token);
            return;
        }

        var username = settings["Username"]?.Trim();
        var password = GetRequiredSetting("AppPassword");
        var security = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
        await client.ConnectAsync(host, port, security, token);
        await client.AuthenticateAsync(string.IsNullOrEmpty(username) ? senderEmail : username, password, token);
    }

    private string GetRequiredSetting(string name)
    {
        var value = settings[name];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Email setting 'EmailSettings:{name}' is missing.");

        return name == "AppPassword" ? value : value.Trim();
    }
}
