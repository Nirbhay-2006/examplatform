using ExamPlatform.API.Models;
using ExamPlatform.API.Repositories;
using Microsoft.Extensions.Logging;

namespace ExamPlatform.API.Services;

public interface IAntiCheatService
{
    Task<int> ComputeCheatingScoreAsync(string violationType, string examId, string studentId);
    bool IsSpeedViolation(DateTime lastActionTime, DateTime currentTime, string questionType);
    Task<bool> ShouldAutoSubmitAsync(string examId, string studentId, int currentScore);
}

public class AntiCheatService : IAntiCheatService
{
    private readonly ISystemConfigRepository _configRepository;
    private readonly ILogger<AntiCheatService> _logger;

    public AntiCheatService(ISystemConfigRepository configRepository, ILogger<AntiCheatService> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    public async Task<int> ComputeCheatingScoreAsync(string violationType, string examId, string studentId)
    {
        var config = await _configRepository.GetConfigAsync();
        var scoreToAdd = 0;

        switch (violationType.ToLower())
        {
            case "tab_switch":
                scoreToAdd = config.AntiCheatConfig.ViolationScoreTabSwitch;
                break;
            case "copy_paste":
                scoreToAdd = config.AntiCheatConfig.ViolationScoreCopyPaste;
                break;
            case "multiple_ip":
                scoreToAdd = config.AntiCheatConfig.ViolationScoreMultipleIP;
                break;
            case "impossible_speed":
                scoreToAdd = config.AntiCheatConfig.ViolationScoreImpossibleSpeed;
                break;
            default:
                scoreToAdd = 5; // Default penalty
                break;
        }

        _logger.LogWarning("Cheating detected: Type={Type}, AddedScore={Score}, User={User}", violationType, scoreToAdd, studentId);
        return scoreToAdd;
    }

    public bool IsSpeedViolation(DateTime lastActionTime, DateTime currentTime, string questionType)
    {
        var timeDiff = (currentTime - lastActionTime).TotalSeconds;
        
        // Impossible speed thresholds
        if (questionType == "mcq" && timeDiff < 2) return true;
        if (questionType == "subjective" && timeDiff < 5) return true;
        
        return false;
    }

    public async Task<bool> ShouldAutoSubmitAsync(string examId, string studentId, int currentScore)
    {
        var config = await _configRepository.GetConfigAsync();
        
        if (config.AntiCheatConfig.ZeroToleranceMode && currentScore > 0)
        {
            _logger.LogWarning("Zero Tolerance Triggered: Auto-submitting exam {ExamId} for user {User}", examId, studentId);
            return true;
        }

        if (currentScore >= config.AntiCheatConfig.MaxCheatingScore)
        {
             _logger.LogWarning("Max Cheating Score Exceeded: Auto-submitting exam {ExamId} for user {User}", examId, studentId);
            return true;
        }

        return false;
    }
}
