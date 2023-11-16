using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Application.Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Auctionify.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : Controller
	{
		private readonly IIdentityService _identityService;
		private readonly SignInWithGoogleOptions _signInWithGoogleOptions;

		public AuthController(IIdentityService identityService, IOptions<SignInWithGoogleOptions> signInWithGoogleOptions)
		{
			_identityService = identityService;
			_signInWithGoogleOptions = signInWithGoogleOptions.Value;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Some properties are not valid.");
			}

			var result = await _identityService.RegisterUserAsync(model);

			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpGet]
		[Route("confirm-email")]
		public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
				return NotFound();

			var result = await _identityService.ConfirmUserEmailAsync(userId, token);

			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok("Confirmed");
		}

		[HttpPost("forget-password")]
		public async Task<IActionResult> ForgetPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
				return NotFound();

			var result = await _identityService.ForgetPasswordAsync(email);

			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Some properties are not valid.");
			}

			var result = await _identityService.ResetPasswordAsync(model);

			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);

		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginViewModel loginModel)
		{
			var result = await _identityService.LoginUserAsync(loginModel);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}


        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserViewModel viewModel)
        {
            var result = await _identityService.AssignRoleToUserAsync(viewModel);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

		[HttpPost("login-with-google")]
		public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
		{
			var settings = new ValidationSettings()
			{
				Audience = new List<string> { _signInWithGoogleOptions.ClientId }
			};

			Payload payload = await ValidateAsync(credential, settings);

			var result = await _identityService.LoginUserWithGoogleAsync(payload);
			
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
