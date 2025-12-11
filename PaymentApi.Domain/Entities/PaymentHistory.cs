using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentApi.Domain.Entities
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
