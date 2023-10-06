using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Auctionify.Infrastructure.Identity
{
    /// <summary>
    /// Concrete implementation of IIdentityService
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public IdentityService(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        public async Task<ResetPasswordResponse> ForgetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email"
                };
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await emailService.SendEmailAsync(email, "Rest Password", "<h1>Follow the instructions to reset your passwrod</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</p>");

            return new ResetPasswordResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has benn sent to the eamil successfully"
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email"
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "Password does not match its confirmation"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new ResetPasswordResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = false

                };

            return new ResetPasswordResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}
