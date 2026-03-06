using System.ComponentModel.DataAnnotations;

namespace NexusPay.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}