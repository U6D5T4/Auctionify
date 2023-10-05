using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
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

        public IdentityService(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<IdentityService> logger,
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.configuration = configuration;
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

            if (user.EmailConfirmed == false)
            {
                return new LoginResponse
                {
                    Errors = new[] { "User is not active" },
                    IsSuccess = false,
                };
            }

            var result = await signInManager.PasswordSignInAsync(user, userModel.Password, false, false);

            if (result.Succeeded)
            {
                logger.LogInformation("User logged in");

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
    }
}
