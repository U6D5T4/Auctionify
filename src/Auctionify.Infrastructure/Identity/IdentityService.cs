using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auctionify.Infrastructure.Identity
{
    /// <summary>
    /// Concrete implementation of IIdentityService
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<IdentityService> logger;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public IdentityService(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<IdentityService> logger,
            IConfiguration configuration,
            IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginViewModel userModel)
        {
            if (userModel == null || string.IsNullOrEmpty(userModel.Email) || string.IsNullOrEmpty(userModel.Password))
            {
                return new LoginResponse
                {
                    Errors = new[] { "User data is emtpy" },
                    IsSuccess = false,
                };
            }
            var user = await userManager.FindByEmailAsync(userModel.Email);

            if (user == null)
            {
                return new LoginResponse
                {
                    Errors = new[] { "User is not found" },
                    IsSuccess = false,
                };
            }

            var result = await signInManager.PasswordSignInAsync(user, userModel.Password, false, false);

            if (result.Succeeded)
            {
                logger.LogInformation("User logged in");
            }
            else
            {
                return new LoginResponse
                {
                    Errors = new[] { "Wrong password or email" },
                    IsSuccess = false,
                };
            }

            if (user.EmailConfirmed == false)
            {
                return new LoginResponse
                {
                    Errors = new[] { "User is not active" },
                    IsSuccess = false,
                };
            }

            var token = await GenerateJWTTokenWithUserClaimsAsync(user);

            return new LoginResponse
            {
                IsSuccess = true,
                Result = token
            };
        }

        /// <summary>
        /// Generation of JWT token with User claims including Email and role
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<TokenModel> GenerateJWTTokenWithUserClaimsAsync(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: configuration["AuthSettings:Issuer"],
                audience: configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenModel
            {
                AccessToken = tokenAsString,
                ExpireDate = token.ValidTo
            };
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

            await emailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</p>");

            return new ResetPasswordResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the eamil successfully"
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
