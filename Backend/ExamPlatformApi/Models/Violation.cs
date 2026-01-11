using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class Violation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("examId")]
    public string ExamId { get; set; } = string.Empty;

    [BsonElement("studentId")]
    public string StudentId { get; set; } = string.Empty;

    [BsonElement("violationType")]
    public string ViolationType { get; set; } = string.Empty; // tab_switch, fullscreen_exit, copy_paste

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
