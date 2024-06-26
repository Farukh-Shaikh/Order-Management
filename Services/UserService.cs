using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, IDataProtector protector, ILogger<UserService> logger)
        {
            _context = context;
            _protector = protector;
            _logger = logger;
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


        public bool IsAboveForty(DateTime? dateOfBirth)
        {
            if (dateOfBirth.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime dob = dateOfBirth.Value;
                int age = now.Year - dob.Year;

                if (age > 40)
                    return true;

                else
                    return false;

            }
            else
            {
                // If date of birth is not provided, consider it as not above 40
                return false;
            }
        }
    }
}
