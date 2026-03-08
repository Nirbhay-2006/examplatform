using ExamNest.Models.DTOs;

namespace ExamNest.Services
{
    public interface IExamAntiCheatingService
    {
        Task<StartExamSessionResponseDto> StartSessionAsync(StartExamSessionRequestDto request, CancellationToken cancellationToken = default);
        Task<LogViolationResponseDto> LogViolationAsync(LogViolationRequestDto request, CancellationToken cancellationToken = default);
        Task<AutoSubmitResponseDto> AutoSubmitAsync(AutoSubmitRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> LogMonitoringEventAsync(MonitoringEventRequestDto request, CancellationToken cancellationToken = default);
        Task<ExamSessionDetailsResponseDto?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ViolationLogResponseDto>> GetViolationsAsync(int? examId, int? userId, DateTime? date, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
