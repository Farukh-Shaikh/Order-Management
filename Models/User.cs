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
        public string? DateOfBirth { get; set; }

        public bool HasConsented { get; set; } // Added consent field

        public DateTime? ConsentDate { get; set; } // Date when consent was given
        public bool IsMarketingConsentGiven { get; set; } // Additional consent field
        public DateTime? LastConsentUpdate { get; set; } // Timestamp for last consent update
    }
}
