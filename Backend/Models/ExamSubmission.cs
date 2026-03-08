using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    public enum SubmissionMode
    {
        Manual = 1,
        AutoViolation = 2
    }

    [Table("exam_submissions")]
    public class ExamSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("session_id")]
        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public ExamSession? Session { get; set; }

        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(255)]
        [Column("reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("mode")]
        public SubmissionMode Mode { get; set; } = SubmissionMode.Manual;

        [Column("answers_snapshot")]
        public string? AnswersSnapshot { get; set; }
    }
}
