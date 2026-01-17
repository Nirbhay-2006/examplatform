using System;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ExamPlatform.API.Helpers;

namespace ExamPlatform.API.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(string toEmail, string name, string otpCode)
    {
        try
        {
            _logger.LogInformation("Attempting to send OTP email to {Email}", toEmail);

            // Validate email settings
            if (string.IsNullOrEmpty(_emailSettings.Password))
            {
                _logger.LogError("Email password is not configured. Please set EmailSettings:Password in user secrets or appsettings.json");
                throw new InvalidOperationException("Email password is not configured. Please configure EmailSettings:Password in user secrets.");
            }

            if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogError("Sender email is not configured");
                throw new InvalidOperationException("Sender email is not configured");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = "Verify Your Email - Exam Platform";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body>
                        <h2>Welcome to Exam Platform!</h2>
                        <p>Hi {name},</p>
                        <p>Your OTP code is: <strong>{otpCode}</strong></p>
                        <p>This code will expire in 10 minutes.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                    </body>
                    </html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("OTP email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}. Error: {ErrorMessage}", toEmail, ex.Message);
            throw; // Re-throw to let the caller handle it
        }
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string name, string otpCode)
    {
        try
        {
            _logger.LogInformation("Attempting to send password reset email to {Email}", toEmail);

            if (string.IsNullOrEmpty(_emailSettings.Password))
            {
                _logger.LogError("Email password is not configured. Please set EmailSettings:Password in user secrets or appsettings.json");
                throw new InvalidOperationException("Email password is not configured. Please configure EmailSettings:Password in user secrets.");
            }

            if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogError("Sender email is not configured");
                throw new InvalidOperationException("Sender email is not configured");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = "Reset Your Password - Exam Platform";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body>
                        <h2>Password Reset Request</h2>
                        <p>Hi {name},</p>
                        <p>We received a request to reset your password.</p>
                        <p>Your password reset code is: <strong>{otpCode}</strong></p>
                        <p>This code will expire in 10 minutes.</p>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                    </body>
                    </html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}. Error: {ErrorMessage}", toEmail, ex.Message);
            throw;
        }
    }
    public async Task SendExamAssignmentEmailAsync(string toEmail, string name, string examTitle, DateTime startTime)
    {
        try
        {
            _logger.LogInformation("Attempting to send exam assignment email to {Email}", toEmail);

            if (string.IsNullOrEmpty(_emailSettings.Password) || string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", toEmail);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = $"Exam Assigned: {examTitle} - Exam Platform";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body>
                        <h2>New Exam Assigned!</h2>
                        <p>Hi {name},</p>
                        <p>You have been assigned to a new exam: <strong>{examTitle}</strong></p>
                        <p><strong>Start Time:</strong> {startTime.ToLocalTime()}</p>
                        <p>Please login to your dashboard to view details.</p>
                        <p>Good luck!</p>
                    </body>
                    </html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Exam assignment email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send exam assignment email to {Email}", toEmail);
        }
    }

    public async Task SendResultPublishedEmailAsync(string toEmail, string name, string examTitle, double percentage)
    {
        try
        {
            _logger.LogInformation("Attempting to send result published email to {Email}", toEmail);

            if (string.IsNullOrEmpty(_emailSettings.Password) || string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", toEmail);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = $"Results Published: {examTitle} - Exam Platform";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body>
                        <h2>Exam Results Available!</h2>
                        <p>Hi {name},</p>
                        <p>The results for <strong>{examTitle}</strong> have been published.</p>
                        <p><strong>Your Score:</strong> {percentage:F2}%</p>
                        <p>Please login to your dashboard to view the full report.</p>
                    </body>
                    </html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Result published email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send result published email to {Email}", toEmail);
        }
    }
}
