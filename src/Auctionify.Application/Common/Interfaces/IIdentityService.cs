using Auctionify.Application.Common.Models.Account;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Auctionify.Application.Common.Interfaces
{
	/// <summary>
	/// Provides an abstraction IdentityService needed for user Authentication/Authorization process
	/// </summary>
	/// Add your corresponding method here like LoginAsync, RegisterAsync and etc.
	public interface IIdentityService
	{
		Task<RegisterResponse> RegisterUserAsync(RegisterViewModel model);

		Task<RegisterResponse> ConfirmUserEmailAsync(string userId, string token);

		Task<ResetPasswordResponse> ForgetPasswordAsync(string email);

		Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel model);

		Task<LoginResponse> LoginUserAsync(LoginViewModel userModel);

		Task<LoginResponse> AssignRoleToUserAsync(string role);

		Task<LoginResponse> LoginUserWithGoogleAsync(Payload payload);
	}
}
