using System;

namespace ExamPlatform.API.Services;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string name, string otpCode);
    Task SendPasswordResetEmailAsync(string toEmail, string name, string otpCode);
    Task SendExamAssignmentEmailAsync(string toEmail, string name, string examTitle, DateTime startTime);
    Task SendResultPublishedEmailAsync(string toEmail, string name, string examTitle, double percentage);
}
