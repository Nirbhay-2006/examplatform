namespace ExamPlatform.API.Services;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string name, string otpCode);
    Task SendPasswordResetEmailAsync(string toEmail, string name, string otpCode);
}
