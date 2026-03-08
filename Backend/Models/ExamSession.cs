using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    public enum SessionStatus
    {
        Active = 1,
        Submitted = 2,
        Terminated = 3
    }

    [Table("exam_sessions")]
    public class ExamSession
    {
        [Key]
        [Column("session_id")]
        public Guid SessionId { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("exam_id")]
        public int ExamId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Required]
        [StringLength(64)]
        [Column("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(1024)]
        [Column("device_info")]
        public string DeviceInfo { get; set; } = string.Empty;

        [Column("status")]
        public SessionStatus Status { get; set; } = SessionStatus.Active;

        [Column("is_violated")]
        public bool IsViolated { get; set; }

        [Column("access_locked_at")]
        public DateTime? AccessLockedAt { get; set; }

        [StringLength(100)]
        [Column("violation_reason")]
        public string? ViolationReason { get; set; }

        public ICollection<ViolationLog> ViolationLogs { get; set; } = new List<ViolationLog>();
        public ICollection<MonitoringLog> MonitoringLogs { get; set; } = new List<MonitoringLog>();
        public ExamSubmission? Submission { get; set; }
    }
}
