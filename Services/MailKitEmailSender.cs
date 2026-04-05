// ASP.NET Core and application dependencies.
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Services
{
    // Sends Identity emails using MailKit SMTP.
    public class MailKitEmailSender : IEmailSender
    {
        // Holds the configured email settings.
        private readonly EmailSettings _emailSettings;

        // Writes diagnostic logs for email sending.
        private readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(
            IOptions<EmailSettings> emailSettings,
            ILogger<MailKitEmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;

            // Override SMTP username from environment variables when available.
            var smtpUserFromEnv = Environment.GetEnvironmentVariable("SMTP_USER");
            if (!string.IsNullOrWhiteSpace(smtpUserFromEnv))
            {
                _emailSettings.SmtpUser = smtpUserFromEnv;
            }

            // Override SMTP password from environment variables when available.
            var smtpPassFromEnv = Environment.GetEnvironmentVariable("SMTP_PASS");
            if (!string.IsNullOrWhiteSpace(smtpPassFromEnv))
            {
                _emailSettings.SmtpPass = smtpPassFromEnv;
            }
        }

        // Sends an HTML email asynchronously.
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Log whether the required SMTP settings are present.
            _logger.LogInformation("SMTP Server set: {HasServer}", !string.IsNullOrWhiteSpace(_emailSettings.SmtpServer));
            _logger.LogInformation("SMTP Port: {Port}", _emailSettings.SmtpPort);
            _logger.LogInformation("SMTP User set: {HasUser}", !string.IsNullOrWhiteSpace(_emailSettings.SmtpUser));
            _logger.LogInformation("SMTP Pass set: {HasPass}", !string.IsNullOrWhiteSpace(_emailSettings.SmtpPass));
            _logger.LogInformation("Sender Email set: {HasSenderEmail}", !string.IsNullOrWhiteSpace(_emailSettings.SenderEmail));

            // Stop if any required email settings are missing.
            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer) ||
                _emailSettings.SmtpPort <= 0 ||
                string.IsNullOrWhiteSpace(_emailSettings.SmtpUser) ||
                string.IsNullOrWhiteSpace(_emailSettings.SmtpPass) ||
                string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings are not configured correctly. Check appsettings.json.");
            }

            // Log the email send attempt.
            _logger.LogInformation("SendEmailAsync called");
            _logger.LogInformation("SMTP Server: {Server}", _emailSettings.SmtpServer);
            _logger.LogInformation("SMTP User: {User}", _emailSettings.SmtpUser);
            _logger.LogInformation("Sender Email: {Sender}", _emailSettings.SenderEmail);
            _logger.LogInformation("Recipient Email: {Recipient}", email);
            _logger.LogInformation("SMTP Pass present: {HasPass}", !string.IsNullOrWhiteSpace(_emailSettings.SmtpPass));

            // Build the email message.
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail));

            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            try
            {
                // Create and use the SMTP client.
                using var smtp = new SmtpClient();

                // Connect to the SMTP server using STARTTLS.
                await smtp.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    SecureSocketOptions.StartTls);

                // Authenticate with the SMTP server.
                await smtp.AuthenticateAsync(
                    _emailSettings.SmtpUser,
                    _emailSettings.SmtpPass);

                // Send the email and close the connection.
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                // Log successful email delivery.
                _logger.LogInformation("Email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                // Log and rethrow any email sending errors.
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw;
            }
        }
    }
}

