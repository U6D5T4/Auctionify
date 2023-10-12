using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
      			_identityService = identityService;
        }

        // api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await identityService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        // api/auth/confirmemail?userid&token
        [HttpGet]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await identityService.ConfirmUserEmailAsync(userId, token);

            if (result.IsSuccess)
                return Ok("Confirmed");

            return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _identityService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _identityService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            var result = await _identityService.LoginUserAsync(loginModel);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
