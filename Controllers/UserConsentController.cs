using Microsoft.AspNetCore.Mvc;
using OrderManagement.Services;

namespace OrderManagement.Controllers
{
    public class ConsentViewModel
    {
        public bool IsConsentGiven { get; set; }
        public bool IsMarketingConsentGiven { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UserConsentController : ControllerBase
    {
        private readonly UserService _userService;

        public UserConsentController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserConsent(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user != null)
            {
                var consentModel = new ConsentViewModel
                {
                    IsConsentGiven = user.HasConsented,
                    IsMarketingConsentGiven = user.IsMarketingConsentGiven
                };

                return Ok(consentModel);
            }

            return NotFound();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUserConsent(int id, [FromBody] ConsentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RequestUserConsentAsync(id, model);
                if (result)
                {
                    return NoContent();
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }
    }
}
