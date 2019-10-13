using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class Transaction
    {
        [Key]
        [Required]
        public int TransactionId { get; set; }

        [Required]
        [Range(-1000,1000)]
        public int Amount { get; set; }


        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public User AccountHolder { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}