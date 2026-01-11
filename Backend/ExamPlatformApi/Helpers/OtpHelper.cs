using System.Security.Cryptography;

namespace ExamPlatform.API.Helpers;

/// <summary>
/// Helper class for generating cryptographically secure OTP codes
/// </summary>
public static class OtpHelper
{
    /// <summary>
    /// Generates a cryptographically secure 6-digit OTP code
    /// </summary>
    /// <returns>A 6-digit OTP code as string</returns>
    public static string GenerateOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var random = BitConverter.ToUInt32(bytes, 0);
        return (random % 900000 + 100000).ToString(); // Ensures 6 digits (100000-999999)
    }
}

