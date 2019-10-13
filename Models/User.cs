using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BankAccounts.Models
{
    public class User
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        [Required]
        public int UserId { get; set; }

        // MySQL VARCHAR and TEXT types can be represeted by a string
        [Required]
        [MinLength(2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [NotMapped]
        [MinLength(8)]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string Confirm { get; set; }

        public List<Transaction> MyTransactions { get; set; }

        // The MySQL DATETIME type can be represented by a DateTime
        [Required]
        public DateTime CreatedAt {get;set;} = DateTime.Now;

        [Required]
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public int Balance { get; set; } = 0;

        // public int GetBalance()
        // {
        //     return balance;
        // }

        // public void SetBalance(int value)
        // {
        //     balance = value;
        // }
    }
}