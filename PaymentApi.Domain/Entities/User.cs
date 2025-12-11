using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentApi.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Login { get; set; } = null!;

        [Required, MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0.00m;

        public int FailedAttempts { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Session>? Sessions { get; set; }
        public ICollection<PaymentHistory>? Payments { get; set; }
    }
}
