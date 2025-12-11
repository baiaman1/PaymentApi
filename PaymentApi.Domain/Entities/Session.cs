using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(500)]
        public string Token { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;

        public User? User { get; set; }
    }
}
