using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusPay.Models
{
    public enum TransactionType
    {
        Transfer,
        Deposit,
        Withdrawal
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderAccountId { get; set; }

        [Required]
        public int ReceiverAccountId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public string Reference { get; set; } = Guid.NewGuid().ToString();

        public TransactionType Type { get; set; } = TransactionType.Transfer;
    }
}