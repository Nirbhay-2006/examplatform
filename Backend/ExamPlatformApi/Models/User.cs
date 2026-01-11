using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "student"; // student, teacher, admin

    [BsonElement("isVerified")]
    public bool IsVerified { get; set; } = false;

    [BsonElement("otpCode")]
    public string? OtpCode { get; set; }

    [BsonElement("otpExpiry")]
    public DateTime? OtpExpiry { get; set; }

    [BsonElement("subscriptionType")]
    public string SubscriptionType { get; set; } = "free"; // free, paid

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
