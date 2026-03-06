using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusPay.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }=String.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        // Optimistic Concurrency Token
        // This prevents two simultaneous transactions from interfering with each other.
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}