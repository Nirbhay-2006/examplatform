using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExamPlatform.API.Models;

public class SystemConfig
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("key")]
    public string Key { get; set; } = "default"; // Singleton key

    [BsonElement("subscriptionLimits")]
    public SubscriptionLimits SubscriptionLimits { get; set; } = new SubscriptionLimits();

    [BsonElement("examRules")]
    public ExamRules ExamRules { get; set; } = new ExamRules();

    [BsonElement("antiCheatConfig")]
    public AntiCheatConfig AntiCheatConfig { get; set; } = new AntiCheatConfig();
}

public class SubscriptionLimits
{
    public int FreePlanStudentLimit { get; set; } = 40;
    public int PremiumPlanStudentLimit { get; set; } = 999999;
    public int FreePlanMaxExams { get; set; } = 5;
}

public class ExamRules
{
    public int MaxDurationMinutes { get; set; } = 180;
    public int MinDurationMinutes { get; set; } = 10;
}

public class AntiCheatConfig
{
    public bool ZeroToleranceMode { get; set; } = false;
    public int MaxCheatingScore { get; set; } = 50;
    public int ViolationScoreTabSwitch { get; set; } = 10;
    public int ViolationScoreCopyPaste { get; set; } = 15;
    public int ViolationScoreImpossibleSpeed { get; set; } = 20;
    public int ViolationScoreMultipleIP { get; set; } = 100;
}
