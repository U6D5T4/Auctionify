using Auctionify.Application.Common.Models.Account;

namespace Auctionify.Application.Common.Interfaces
{
    /// <summary>
    /// Provides an abstraction IdentityService needed for user Authentication/Authorization process
    /// </summary>
    /// Add your corresponding method here like LoginAsync, RegisterAsync and etc.
    public interface IIdentityService
    {
        Task<ResetPasswordResponse> ForgetPasswordAsync(string email);

        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel model);

        Task<LoginResponse> LoginUserAsync(LoginViewModel userModel);
    }
}
