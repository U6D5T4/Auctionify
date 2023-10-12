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

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _identityService.RegisterUserAsync(model);

				if (result.IsSuccess)
					return Ok(result);
				return BadRequest(result);
			}

			return BadRequest("Some properties are not valid.");
		}


		[HttpGet]
		[Route("confirm-email")]
		public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
				return NotFound();

			var result = await _identityService.ConfirmUserEmailAsync(userId, token);

			if (result.IsSuccess)
				return Ok("Confirmed");

			return BadRequest(result);
		}

		[HttpPost("forget-password")]
		public async Task<IActionResult> ForgetPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
				return NotFound();

			var result = await _identityService.ForgetPasswordAsync(email);

			if (result.IsSuccess)
				return Ok(result);

			return BadRequest(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _identityService.ResetPasswordAsync(model);

				if (result.IsSuccess)
					return Ok(result);

				return BadRequest(result);
			}

			return BadRequest("Some properties are not valid.");
		}

		[HttpPost("login")]
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
