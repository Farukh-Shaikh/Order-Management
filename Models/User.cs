using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        public bool HasConsented { get; set; } // Added consent field

        //Added comment to test pre-hook 
        //Added another comment to test pre-hook 
        //Added more comment to test pre-hook 
        //Added more comment to test pre-hook 
        //Added more comment to test pre-hook 
    }
}
