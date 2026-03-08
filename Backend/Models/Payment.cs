using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamNest.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string? PaymentGateway { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
