using MongoDB.Driver;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Services;

public interface IAuditService
{
    Task LogAsync(string action, string userId, string details, string ipAddress);
    Task<List<AuditLog>> GetLogsAsync(int limit = 100);
}

public class AuditService : IAuditService
{
    private readonly IMongoCollection<AuditLog> _auditLogs;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IMongoDatabase database, ILogger<AuditService> logger)
    {
        _auditLogs = database.GetCollection<AuditLog>("audit_logs");
        _logger = logger;
    }

    public async Task LogAsync(string action, string userId, string details, string ipAddress)
    {
        try
        {
            var log = new AuditLog
            {
                Action = action,
                UserId = userId,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
            await _auditLogs.InsertOneAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log: {Action}", action);
            // Non-blocking: Audit logging failure shouldn't crash the request
        }
    }

    public async Task<List<AuditLog>> GetLogsAsync(int limit = 100)
    {
        return await _auditLogs.Find(_ => true)
            .SortByDescending(l => l.Timestamp)
            .Limit(limit)
            .ToListAsync();
    }
}
