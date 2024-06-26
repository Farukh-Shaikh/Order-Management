﻿using Microsoft.AspNetCore.Mvc;
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

    }
}
