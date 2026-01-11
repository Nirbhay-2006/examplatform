using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class Response
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("examId")]
    public string ExamId { get; set; } = string.Empty;

    [BsonElement("studentId")]
    public string StudentId { get; set; } = string.Empty;

    [BsonElement("answers")]
    public List<Answer> Answers { get; set; } = new();

    [BsonElement("startedAt")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("submittedAt")]
    public DateTime? SubmittedAt { get; set; }

    [BsonElement("violationCount")]
    public int ViolationCount { get; set; } = 0;

    [BsonElement("isAutoSubmitted")]
    public bool IsAutoSubmitted { get; set; } = false;

    [BsonElement("status")]
    public string Status { get; set; } = "in_progress"; // in_progress, submitted, evaluated
}

public class Answer
{
    [BsonElement("questionId")]
    public string QuestionId { get; set; } = string.Empty;

    [BsonElement("answer")]
    public string AnswerText { get; set; } = string.Empty;

    [BsonElement("isCorrect")]
    public bool? IsCorrect { get; set; }

    [BsonElement("marksObtained")]
    public int MarksObtained { get; set; } = 0;
}
