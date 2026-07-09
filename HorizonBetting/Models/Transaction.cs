using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorizonBetting.Models
{
    public enum TransactionType
    {
        Debit,
        Credit
    }

    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Display(Name = "Capture Date")]
        [DataType(DataType.DateTime)]
        public DateTime CaptureDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public TransactionType Type { get; set; }

        // Foreign Key
        public int AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
