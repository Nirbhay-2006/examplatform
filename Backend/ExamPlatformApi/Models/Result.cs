using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class Result
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("examId")]
    public string ExamId { get; set; } = string.Empty;

    [BsonElement("studentId")]
    public string StudentId { get; set; } = string.Empty;

    [BsonElement("responseId")]
    public string ResponseId { get; set; } = string.Empty;

    [BsonElement("totalMarks")]
    public int TotalMarks { get; set; }

    [BsonElement("marksObtained")]
    public int MarksObtained { get; set; }

    [BsonElement("percentage")]
    public double Percentage { get; set; }

    [BsonElement("isPassed")]
    public bool IsPassed { get; set; }

    [BsonElement("publishedAt")]
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
