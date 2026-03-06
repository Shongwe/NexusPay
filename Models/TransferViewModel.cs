using System.ComponentModel.DataAnnotations;

namespace NexusPay.ViewModels
{
    public class TransferViewModel
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string Description { get; set; }=String.Empty;
    }
}