using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    [Table("course_exams")]
    public class CourseExam
    {
        [Key]
        [Column("course_exam_id")]
        public int CourseExamId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
    }
}
