using Auctionify.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using Auctionify.Core.Entities;
using Auctionify.Application.Common.Models.Account;

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
        public async Task<RegisterResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new NullReferenceException("Register Model is null");

            if (model.Password != model.ConfirmPassword)
                return new RegisterResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                // usually browser can't handle special characters in url, so we need to encode the token
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);

                // we need to encode the token to base64 so that we can pass it in the url
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                var url = $"{configuration["AppUrl"]}/api/v1/auth/confirmemail?userid={user.Id}&token={validEmailToken}";

                await emailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to Auctionify</h1>" +
                                        $"<p>Please confirm your email by <a href='{url}'>clicking here</a></p>");

                return new RegisterResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new RegisterResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };

        }

        public async Task<RegisterResponse> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new RegisterResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                };

            return new RegisterResponse
            {
                Message = "Email did not confirm",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
