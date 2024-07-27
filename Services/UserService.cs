using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderManagement.Controllers;
using OrderManagement.Models;
using OrderManagement.Repositories;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagement.Services
{
    
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;
        private readonly ILogger<UserService> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _encryptionKey;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger, IDataProtectionProvider provider, SmtpClient smtpClient, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _protector = provider.CreateProtector("UserDataProtector");
            _smtpClient = smtpClient; // Ensure SMTP client is configured properly
            _encryptionKey = configuration["EncryptionKey"]; // Retrieve encryption key from configuration
        }

        public (bool IsSuccess, User User, string ErrorMessage) GetUserById(int id)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user != null && user.HasConsented)
                {
                    user.Email = _protector.Unprotect(user.Email);
                    return (true, user, null);
                }
                return (false, null, "User not found or has not consented.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID");
                return (false, null, "An error occurred while fetching the user.");
            }
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


        private string Encrypt(string data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = new byte[16]; // Initialization vector with zeroes

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                return Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));
            }
        }

        private string Decrypt(string encryptedData)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = new byte[16]; // Initialization vector with zeroes

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] encryptedDataBytes = Convert.FromBase64String(encryptedData);

                return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(encryptedDataBytes, 0, encryptedDataBytes.Length));
            }
        }

        private async Task<User> GetUserIfConsentedAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.HasConsented)
            {
                return user;
            }

            if (user != null)
            {
                _logger.LogWarning($"Attempted to access user data without consent: UserId={id}");
            }
            else
            {
                _logger.LogWarning($"User not found: UserId={id}");
            }

            return null;
        }

        public async Task<bool> RequestUserConsentAsync(int userId, ConsentViewModel model)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.HasConsented = model.IsConsentGiven;
                user.ConsentDate = model.IsConsentGiven ? DateTime.UtcNow : (DateTime?)null;
                user.IsMarketingConsentGiven = model.IsMarketingConsentGiven;
                user.LastConsentUpdate = DateTime.UtcNow; // Record timestamp of consent update

                await _context.SaveChangesAsync();

                // Log the consent action
                _logger.LogInformation($"User consent updated: UserId={userId}, ConsentGiven={model.IsConsentGiven}, MarketingConsentGiven={model.IsMarketingConsentGiven}");

                return true;
            }

            return false;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await GetUserIfConsentedAsync(id);

            if (user != null && user.Email != null)
            {
                user.Email = _protector.Unprotect(user.Email);
                user.DateOfBirth = Decrypt(user.DateOfBirth); // Decrypt DateOfBirth
            }

            return user;
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var user = await GetUserIfConsentedAsync(updatedUser.Id);

            if (user != null)
            {
                user.FullName = updatedUser.FullName;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);
                user.Email = _protector.Protect(updatedUser.Email);
                user.DateOfBirth = Encrypt(updatedUser.DateOfBirth); // Encrypt DateOfBirth

                await _context.SaveChangesAsync();

                // Log the update operation
                _logger.LogInformation($"User updated: UserId={user.Id}");
            }
            else
            {
                _logger.LogWarning($"Attempted to update user without consent: UserId={updatedUser.Id}");
                throw new InvalidOperationException("User consent is required before updating data.");
            }
        }

        public async Task SaveUserAsync(User user)
        {
            if (user.HasConsented)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.Email = _protector.Protect(user.Email);
                user.DateOfBirth = Encrypt(user.DateOfBirth); // Encrypt DateOfBirth
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Log the save operation
                _logger.LogInformation($"User saved: UserId={user.Id}, ConsentGiven={user.HasConsented}");
            }
            else
            {
                _logger.LogWarning($"Attempted to save user without consent: UserId={user.Id}");
                throw new InvalidOperationException("User consent is required before saving data.");
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserIfConsentedAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                // Log the delete operation
                _logger.LogInformation($"User deleted: UserId={id}");
                return true;
            }

            _logger.LogWarning($"Attempted to delete non-existent or non-consented user: UserId={id}");
            return false;
        }

        public static bool IsAboveForty(DateTime? dateOfBirth)
        {
            if (dateOfBirth.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime dob = dateOfBirth.Value;
                int age = now.Year - dob.Year;

                if (dob.Date > now.AddYears(-age))
                    age--;

                return age > 40;
            }
            return false;
        }

    }
}
