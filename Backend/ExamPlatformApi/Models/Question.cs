using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("examId")]
    public string ExamId { get; set; } = string.Empty;

    [BsonElement("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("questionType")]
    public string QuestionType { get; set; } = "mcq"; // mcq, text

    [BsonElement("options")]
    public List<string> Options { get; set; } = new();

    [BsonElement("correctAnswer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [BsonElement("marks")]
    public int Marks { get; set; }

    [BsonElement("order")]
    public int Order { get; set; }
}
