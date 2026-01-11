using ExamPlatform.API.Models;

namespace ExamPlatform.API.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
