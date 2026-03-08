using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    [Table("monitoring_logs")]
    public class MonitoringLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("session_id")]
        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public ExamSession? Session { get; set; }

        [Required]
        [StringLength(100)]
        [Column("event_type")]
        public string EventType { get; set; } = string.Empty;

        [Column("event_time")]
        public DateTime EventTime { get; set; } = DateTime.UtcNow;

        [StringLength(2048)]
        [Column("details")]
        public string? Details { get; set; }
    }
}
