using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int OrderId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
