using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Models;
using OrderManagement.Services;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }


        //create apit to delete cardholder data
        [HttpDelete("DeleteCardholderData")]
        public async Task<IActionResult> DeleteCardholderData(int id)
        {
            try
            {
                await _accountService.DeleteCardholderDataAsync(id);
                return Ok("Cardholder data deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //create api to add cardholder 
        [HttpPost("AddCardholderData")]
        public async Task<IActionResult> AddCardholderData(Cardholder cardholder)
        {
            try
            {
                await _accountService.AddCardholderDataAsync(cardholder);
                return Ok("Cardholder data added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
