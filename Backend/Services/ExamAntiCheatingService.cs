using ExamNest.Data;
using ExamNest.Models;
using ExamNest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ExamNest.Services
{
    public class ExamAntiCheatingService : IExamAntiCheatingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamAntiCheatingService> _logger;

        public ExamAntiCheatingService(AppDbContext context, ILogger<ExamAntiCheatingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StartExamSessionResponseDto> StartSessionAsync(StartExamSessionRequestDto request, CancellationToken cancellationToken = default)
        {
            var hasActiveSession = await _context.ExamSessions
                .AnyAsync(s => s.UserId == request.UserId && s.ExamId == request.ExamId && s.Status == SessionStatus.Active, cancellationToken);

            if (hasActiveSession)
            {
                throw new InvalidOperationException("An active session already exists for this user and exam.");
            }

            var session = new ExamSession
            {
                UserId = request.UserId,
                ExamId = request.ExamId,
                DeviceInfo = request.DeviceInfo,
                IpAddress = request.IpAddress,
                StartTime = DateTime.UtcNow,
                Status = SessionStatus.Active,
                IsViolated = false
            };

            _context.ExamSessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            return new StartExamSessionResponseDto
            {
                SessionId = session.SessionId,
                StartTime = session.StartTime,
                SessionStatus = ToSnakeCaseStatus(session.Status)
            };
        }

        public async Task<LogViolationResponseDto> LogViolationAsync(LogViolationRequestDto request, CancellationToken cancellationToken = default)
        {
            var session = await _context.ExamSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId, cancellationToken)
                ?? throw new KeyNotFoundException("Session not found.");

            var violationType = ParseViolationType(request.ViolationType);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var now = DateTime.UtcNow;
            var actionTaken = "violation_logged_only";

            _context.ViolationLogs.Add(new ViolationLog
            {
                SessionId = session.SessionId,
                ViolationType = violationType,
                Timestamp = now,
                ActionTaken = session.Status == SessionStatus.Active ? "auto_submitted" : "session_already_closed"
            });

            _context.MonitoringLogs.Add(new MonitoringLog
            {
                SessionId = session.SessionId,
                EventType = "violation_detected",
                EventTime = now,
                Details = request.ViolationType
            });

            if (session.Status == SessionStatus.Active)
            {
                var reason = $"Violation detected: {request.ViolationType}";
                await InternalAutoSubmitAsync(session, reason, SubmissionMode.AutoViolation, null, now, cancellationToken);
                actionTaken = "auto_submitted";
            }
            else
            {
                actionTaken = "session_already_closed";
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogWarning("Violation handled. SessionId: {SessionId}, Type: {ViolationType}, Action: {Action}",
                session.SessionId, request.ViolationType, actionTaken);

            return new LogViolationResponseDto
            {
                ViolationLogged = true,
                ActionTaken = actionTaken
            };
        }

        public async Task<AutoSubmitResponseDto> AutoSubmitAsync(AutoSubmitRequestDto request, CancellationToken cancellationToken = default)
        {
            var session = await _context.ExamSessions
                .Include(s => s.Submission)
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId, cancellationToken)
                ?? throw new KeyNotFoundException("Session not found.");

            if (session.Status != SessionStatus.Active)
            {
                var submittedAt = session.Submission?.SubmittedAt ?? session.EndTime ?? DateTime.UtcNow;
                var existingReason = session.Submission?.Reason ?? request.Reason;

                return new AutoSubmitResponseDto
                {
                    Submitted = true,
                    SessionStatus = ToSnakeCaseStatus(session.Status),
                    SubmittedAt = submittedAt,
                    Reason = existingReason
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var now = DateTime.UtcNow;
            await InternalAutoSubmitAsync(session, request.Reason, SubmissionMode.AutoViolation, request.AnswersSnapshot, now, cancellationToken);

            _context.MonitoringLogs.Add(new MonitoringLog
            {
                SessionId = session.SessionId,
                EventType = "auto_submit",
                EventTime = now,
                Details = request.Reason
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AutoSubmitResponseDto
            {
                Submitted = true,
                SessionStatus = ToSnakeCaseStatus(session.Status),
                SubmittedAt = now,
                Reason = request.Reason
            };
        }

        public async Task<bool> LogMonitoringEventAsync(MonitoringEventRequestDto request, CancellationToken cancellationToken = default)
        {
            var exists = await _context.ExamSessions
                .AnyAsync(s => s.SessionId == request.SessionId, cancellationToken);

            if (!exists)
            {
                throw new KeyNotFoundException("Session not found.");
            }

            _context.MonitoringLogs.Add(new MonitoringLog
            {
                SessionId = request.SessionId,
                EventType = request.EventType,
                EventTime = DateTime.UtcNow,
                Details = request.Details
            });

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ExamSessionDetailsResponseDto?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = await _context.ExamSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionId == sessionId, cancellationToken);

            if (session == null)
            {
                return null;
            }

            return new ExamSessionDetailsResponseDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                ExamId = session.ExamId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Status = ToSnakeCaseStatus(session.Status),
                IsViolated = session.IsViolated,
                ViolationReason = session.ViolationReason,
                IpAddress = session.IpAddress,
                DeviceInfo = session.DeviceInfo
            };
        }

        public async Task<IReadOnlyList<ViolationLogResponseDto>> GetViolationsAsync(
            int? examId,
            int? userId,
            DateTime? date,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ViolationLogs
                .AsNoTracking()
                .Include(v => v.Session)
                .AsQueryable();

            if (examId.HasValue)
            {
                query = query.Where(v => v.Session != null && v.Session.ExamId == examId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(v => v.Session != null && v.Session.UserId == userId.Value);
            }

            if (date.HasValue)
            {
                var dayStart = date.Value.Date;
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(v => v.Timestamp >= dayStart && v.Timestamp < dayEnd);
            }

            var skip = Math.Max(0, page - 1) * pageSize;

            return await query
                .OrderByDescending(v => v.Timestamp)
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new ViolationLogResponseDto
                {
                    Id = v.Id,
                    SessionId = v.SessionId,
                    UserId = v.Session != null ? v.Session.UserId : 0,
                    ExamId = v.Session != null ? v.Session.ExamId : 0,
                    ViolationType = ToSnakeCaseViolation(v.ViolationType),
                    Timestamp = v.Timestamp,
                    ActionTaken = v.ActionTaken
                })
                .ToListAsync(cancellationToken);
        }

        private async Task InternalAutoSubmitAsync(
            ExamSession session,
            string reason,
            SubmissionMode mode,
            string? answersSnapshot,
            DateTime now,
            CancellationToken cancellationToken)
        {
            session.IsViolated = mode == SubmissionMode.AutoViolation || session.IsViolated;
            session.ViolationReason = Truncate(reason, 100);
            session.AccessLockedAt = now;
            session.EndTime = now;
            session.Status = SessionStatus.Submitted;

            var existingSubmission = await _context.ExamSubmissions
                .FirstOrDefaultAsync(s => s.SessionId == session.SessionId, cancellationToken);

            if (existingSubmission == null)
            {
                _context.ExamSubmissions.Add(new ExamSubmission
                {
                    SessionId = session.SessionId,
                    SubmittedAt = now,
                    Reason = reason,
                    Mode = mode,
                    AnswersSnapshot = answersSnapshot
                });
            }
            else if (!string.IsNullOrWhiteSpace(answersSnapshot) && string.IsNullOrWhiteSpace(existingSubmission.AnswersSnapshot))
            {
                existingSubmission.AnswersSnapshot = answersSnapshot;
            }
        }

        private static ViolationType ParseViolationType(string violationType)
        {
            return violationType.Trim().ToLowerInvariant() switch
            {
                "tab_switch" => ViolationType.TabSwitch,
                "focus_loss" => ViolationType.FocusLoss,
                "refresh" => ViolationType.Refresh,
                "fullscreen_exit" => ViolationType.FullscreenExit,
                "copy_paste" => ViolationType.CopyPaste,
                "multiple_login" => ViolationType.MultipleLogin,
                _ => throw new ArgumentOutOfRangeException(nameof(violationType), "Unsupported violation_type.")
            };
        }

        private static string ToSnakeCaseStatus(SessionStatus status)
        {
            return status switch
            {
                SessionStatus.Active => "active",
                SessionStatus.Submitted => "submitted",
                SessionStatus.Terminated => "terminated",
                _ => "active"
            };
        }

        private static string ToSnakeCaseViolation(ViolationType type)
        {
            return type switch
            {
                ViolationType.TabSwitch => "tab_switch",
                ViolationType.FocusLoss => "focus_loss",
                ViolationType.Refresh => "refresh",
                ViolationType.FullscreenExit => "fullscreen_exit",
                ViolationType.CopyPaste => "copy_paste",
                ViolationType.MultipleLogin => "multiple_login",
                _ => "unknown"
            };
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value[..maxLength];
        }
    }
}
