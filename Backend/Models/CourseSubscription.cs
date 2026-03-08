using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    [Table("course_subscriptions")]
    public class CourseSubscription
    {
        [Key]
        [Column("subscription_id")]
        public int SubscriptionId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("subscribed_at")]
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        [ForeignKey(nameof(StudentId))]
        public User? Student { get; set; }
    }
}
