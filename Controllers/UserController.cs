using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using OrderManagement.Services;
using System.Net.NetworkInformation;

namespace OrderManagement.Controllers
{

   

    [ApiController]
    [Route("[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService) { 
            _userService = userService;
        }

        //create a api to get user consent
        [HttpPost("GetUserConsent")]
        public IActionResult GetUserConsent(User user)
        {
            return Ok(_userService.GetUserConsent(user));
        }

        //create api to save user
        [HttpPost("SaveUser")]
        public IActionResult SaveUser(User user)
        {
            try
            {
                _userService.SaveUser(user);
                return Ok("User saved successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //create api to check if user is above forty age
        [HttpGet("IsUserAboveForty")]
        public IActionResult IsAboveForty(DateTime? dateOfBirth)
        {
            return Ok(_userService.IsAboveForty(dateOfBirth));
        }


        //create api to handle data breach
        [HttpPost("HandleDataBreach")]
        public IActionResult HandleDataBreach(String breachDetails)
        {
            try
            {
                _userService.HandleDataBreach(breachDetails);
                return Ok("Data breach handled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
