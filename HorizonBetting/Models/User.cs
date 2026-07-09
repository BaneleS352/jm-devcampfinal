using System.ComponentModel.DataAnnotations;

namespace HorizonBetting.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID Number must be exactly 13 digits.")]
        public string IdNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
