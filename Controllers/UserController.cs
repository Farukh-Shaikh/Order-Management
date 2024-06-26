using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using OrderManagement.Models;
using OrderManagement.Services;

namespace OrderManagement.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService) {
            _userService = userService;
        }

        [HttpGet("IsUserAboveForty")]
        public IActionResult IsAboveForty(DateTime? dateOfBirth)
        {
            //checking if user is above 40
            if (dateOfBirth.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime dob = dateOfBirth.Value;
                int age = now.Year - dob.Year;

                if (age > 40)
                    return Ok();

                else
                    return BadRequest();

            }
            else
            {
                return Ok();
            }

        }
    }
}
