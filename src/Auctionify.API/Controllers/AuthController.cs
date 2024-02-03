using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Application.Common.Options;
using Microsoft.AspNetCore.Authorization;
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
		private readonly ICurrentUserService _currentUserService;

		public AuthController(
			IIdentityService identityService,
			IOptions<SignInWithGoogleOptions> signInWithGoogleOptions,
			ICurrentUserService currentUserService
		)
		{
			_identityService = identityService;
			_signInWithGoogleOptions = signInWithGoogleOptions.Value;
			_currentUserService = currentUserService;
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
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
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
		[Authorize]
		public async Task<IActionResult> AssignRole(string role)
		{
			var result = await _identityService.AssignRoleToUserAsync(role);

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

		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Some properties are not valid.");
			}

			var email = _currentUserService.UserEmail;

			var result = await _identityService.ChangeUserPasswordAsync(email, model);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok();
		}

		[HttpGet("google-client-id")]
		public IActionResult GetGoogleClientId()
		{
			return Ok(_signInWithGoogleOptions.ClientId);
		}

		[Authorize]
		[HttpPost("login-with-selected-role")]
		public async Task<IActionResult> CheckEligibilityToLoginWithSelectedRole([FromForm] string role)
		{
			var result = await _identityService.CheckEligibilityToLoginWithSelectedRole(role);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[Authorize]
		[HttpPost("create-new-user-role")]
		public async Task<IActionResult> CreateNewUserWithNewRole([FromForm] string role)
		{
			var result = await _identityService.CreateNewUserWithNewRole(role);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
