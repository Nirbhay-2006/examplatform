using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExamNest.Models.DTOs
{
    public class StartExamSessionRequestDto
    {
        [JsonPropertyName("user_id")]
        [Required]
        public int UserId { get; set; }

        [JsonPropertyName("exam_id")]
        [Required]
        public int ExamId { get; set; }

        [JsonPropertyName("device_info")]
        [Required]
        [StringLength(1024)]
        public string DeviceInfo { get; set; } = string.Empty;

        [JsonPropertyName("ip_address")]
        [Required]
        [StringLength(64)]
        public string IpAddress { get; set; } = string.Empty;
    }

    public class StartExamSessionResponseDto
    {
        [JsonPropertyName("session_id")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("session_status")]
        public string SessionStatus { get; set; } = string.Empty;
    }

    public class LogViolationRequestDto
    {
        [JsonPropertyName("session_id")]
        [Required]
        public Guid SessionId { get; set; }

        [JsonPropertyName("violation_type")]
        [Required]
        [RegularExpression("tab_switch|focus_loss|refresh|fullscreen_exit|copy_paste|multiple_login")]
        public string ViolationType { get; set; } = string.Empty;
    }

    public class LogViolationResponseDto
    {
        [JsonPropertyName("violation_logged")]
        public bool ViolationLogged { get; set; }

        [JsonPropertyName("action_taken")]
        public string ActionTaken { get; set; } = string.Empty;
    }

    public class AutoSubmitRequestDto
    {
        [JsonPropertyName("session_id")]
        [Required]
        public Guid SessionId { get; set; }

        [JsonPropertyName("reason")]
        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("answers_snapshot")]
        public string? AnswersSnapshot { get; set; }
    }

    public class AutoSubmitResponseDto
    {
        [JsonPropertyName("submitted")]
        public bool Submitted { get; set; }

        [JsonPropertyName("session_status")]
        public string SessionStatus { get; set; } = string.Empty;

        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }

    public class MonitoringEventRequestDto
    {
        [JsonPropertyName("session_id")]
        [Required]
        public Guid SessionId { get; set; }

        [JsonPropertyName("event_type")]
        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty;

        [JsonPropertyName("details")]
        [StringLength(2048)]
        public string? Details { get; set; }
    }

    public class ExamSessionDetailsResponseDto
    {
        [JsonPropertyName("session_id")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("exam_id")]
        public int ExamId { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime? EndTime { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("is_violated")]
        public bool IsViolated { get; set; }

        [JsonPropertyName("violation_reason")]
        public string? ViolationReason { get; set; }

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [JsonPropertyName("device_info")]
        public string DeviceInfo { get; set; } = string.Empty;
    }

    public class ViolationLogResponseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("session_id")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("exam_id")]
        public int ExamId { get; set; }

        [JsonPropertyName("violation_type")]
        public string ViolationType { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("action_taken")]
        public string ActionTaken { get; set; } = string.Empty;
    }
}
