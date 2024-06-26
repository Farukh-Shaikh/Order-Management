using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Controllers;
using OrderManagement.Models;
using OrderManagement.Repositories;
using System.Net;
using System.Net.Mail;


namespace OrderManagement.Services
{
    public class UserService
    {
        //Testing pipeline check

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IDataProtector _protector;
        private readonly SmtpClient _smtpClient;

        public UserService(ApplicationDbContext context, ILogger<UserController> logger, IDataProtectionProvider provider)
        {
            _context = context;
            _logger = logger;
            _protector = provider.CreateProtector("UserDataProtector");
            _smtpClient = new SmtpClient("smtp.example.com") // Configure your SMTP client
            {
                Port = 587, // Set your SMTP port
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                EnableSsl = true
            };
        }
        public void RequestUserConsent(User user)
        {
            //setting user consent from frontend
            user.HasConsented = true;

        }

        public bool GetUserConsent(User user)
        {
            //returning user consent
            if (user.HasConsented)
            {
                return true;
            }
            return false;
        }


        public void SaveUser(User user)
        {
            if (user.HasConsented)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.Email = _protector.Protect(user.Email);
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("User consent is required before saving data.");
            }
        }

        public bool IsAboveForty(DateTime? dateOfBirth)
        {
            if (dateOfBirth.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime dob = dateOfBirth.Value;
                int age = now.Year - dob.Year;
                // Check if the user has already had their birthday this year
                if (dob.Date > now.AddYears(-age))
                    age--;
                return age > 40;
            }
            else
            {
                // If date of birth is not provided, consider it as not above 40
                return false;
            }
        }

        public void HandleDataBreach(string breachDetails)
        {
            _logger.LogError($"Data breach detected: {breachDetails}");
            NotifyAuthorities(breachDetails);
            NotifyUsers();
        }

        private void NotifyAuthorities(string breachDetails)
        {
            var authorityEmail = "authority@example.com";
            var mailMessage = new MailMessage("no-reply@example.com", authorityEmail)
            {
                Subject = "Data Breach Notification",
                Body = $"A data breach has been detected:\n\n{breachDetails}"
            };
            _smtpClient.Send(mailMessage);
        }

        private void NotifyUsers()
        {
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var userEmail = _protector.Unprotect(user.Email);
                var mailMessage = new MailMessage("no-reply@example.com", userEmail)
                {
                    Subject = "Important: Data Breach Notification",
                    Body = "Dear user,\n\nWe regret to inform you that a data breach has occurred. Please take necessary precautions to protect your personal information."
                };
                _smtpClient.Send(mailMessage);
            }
        }
    }
}
