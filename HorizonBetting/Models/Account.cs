using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorizonBetting.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; } = string.Empty;

        [Display(Name = "Outstanding Balance")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OutstandingBalance { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User? User { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
