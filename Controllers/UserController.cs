using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Controllers
{
    public class UserController : ControllerBase
    {
        [HttpGet("GetNames")]
        public IActionResult GetNames()
        {
            return Ok();
        }


    }
}
