using Auctionify.Application.Common.Models.Account;

namespace Auctionify.Application.Common.Interfaces
{
    /// <summary>
    /// Provides an abstraction IdentityService needed for user Authentication/Authorization process
    /// </summary>
    /// Add your corresponding method here like LoginAsync, RegisterAsync and etc.
    public interface IIdentityService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> ConfirmUserEmailAsync(string userId, string token); // why namely userId and token? because we need to confirm the email of a specific user
    }
}
