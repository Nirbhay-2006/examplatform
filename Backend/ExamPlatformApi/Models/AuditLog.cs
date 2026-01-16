using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class AuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty; // e.g., "LOGIN", "CREATE_EXAM", "UPDATE_SETTINGS"

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("details")]
    public string Details { get; set; } = string.Empty;

    [BsonElement("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
