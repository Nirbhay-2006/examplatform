using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class Exam
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("teacherId")]
    public string TeacherId { get; set; } = string.Empty;

    [BsonElement("duration")]
    public int Duration { get; set; } // in minutes

    [BsonElement("totalMarks")]
    public int TotalMarks { get; set; }

    [BsonElement("passingMarks")]
    public int PassingMarks { get; set; }

    [BsonElement("startTime")]
    public DateTime StartTime { get; set; }

    [BsonElement("endTime")]
    public DateTime EndTime { get; set; }

    [BsonElement("isPaid")]
    public bool IsPaid { get; set; } = false;

    [BsonElement("isPublished")]
    public bool IsPublished { get; set; } = false;

    [BsonElement("maxViolations")]
    public int MaxViolations { get; set; } = 3;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("assignedStudents")]
    public List<string> AssignedStudents { get; set; } = new();
}
