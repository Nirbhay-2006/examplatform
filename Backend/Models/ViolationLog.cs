using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    public enum ViolationType
    {
        TabSwitch = 1,
        FocusLoss = 2,
        Refresh = 3,
        FullscreenExit = 4,
        CopyPaste = 5,
        MultipleLogin = 6
    }

    [Table("violation_logs")]
    public class ViolationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("session_id")]
        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public ExamSession? Session { get; set; }

        [Column("violation_type")]
        public ViolationType ViolationType { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        [Column("action_taken")]
        public string ActionTaken { get; set; } = "auto_submitted";
    }
}
