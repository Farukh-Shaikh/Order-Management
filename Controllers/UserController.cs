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

        [HttpGet("GetNames")]
        public IActionResult GetNames()
        {
            return Ok();
        }

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int id)
        {
            var userResult = _userService.GetUserById(id);
            if (userResult.IsSuccess)
            {
                return Ok(userResult.User);
            }
            return BadRequest(userResult.ErrorMessage);
        }

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int id)
        {
            var userResult = _userService.GetUserById(id);
            if (userResult.IsSuccess)
            {
                return Ok(userResult.User);
            }
            return BadRequest(userResult.ErrorMessage);
        }

        [HttpPost("GetUserConsent")]
        public IActionResult GetUserConsent(User user)
        {
            return Ok(_userService.GetUserConsent(user));
        }

        [HttpGet("Calculate")]
        public IActionResult Calculate()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        if (k % 2 == 0)
                        {
                            return Ok();
                        }
                    }
                }
            }

            return BadRequest();
        }
    }
}
