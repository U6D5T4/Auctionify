using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Common.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using AccountRole = Auctionify.Core.Enums.AccountRole;

namespace Auctionify.Infrastructure.Identity
{
	/// <summary>
	/// Concrete implementation of IIdentityService
	/// </summary>
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ILogger<IdentityService> _logger;
		private readonly IEmailService _emailService;
		private readonly RoleManager<Role> _roleManager;
		private readonly AuthSettingsOptions _authSettingsOptions;
		private readonly AppOptions _appOptions;
		private readonly ICurrentUserService _currentUserService;
		private readonly IUserRoleDbContextService _userRoleDbContextService;

		public IdentityService(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			ILogger<IdentityService> logger,
			IEmailService emailService,
			RoleManager<Role> roleManager,
			IOptions<AuthSettingsOptions> authSettingsOptions,
			IOptions<AppOptions> appOptions,
			ICurrentUserService currentUserService,
			IUserRoleDbContextService userRoleDbContextService
		)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailService = emailService;
			_roleManager = roleManager;
			_authSettingsOptions = authSettingsOptions.Value;
			_appOptions = appOptions.Value;
			_currentUserService = currentUserService;
			_userRoleDbContextService = userRoleDbContextService;
		}

		public async Task<LoginResponse> LoginUserAsync(LoginViewModel loginModel)
		{
			if (
				loginModel is null
				|| string.IsNullOrEmpty(loginModel.Email)
				|| string.IsNullOrEmpty(loginModel.Password)
			)
			{
				return new LoginResponse
				{
					Errors = new[] { "User data is empty" },
					IsSuccess = false,
				};
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == loginModel.Email && !u.IsDeleted
			);

			if (user is null)
			{
				return new LoginResponse
				{
					Errors = new[] { "User is not found or deleted" },
					IsSuccess = false,
				};
			}

			var result = await _signInManager.PasswordSignInAsync(
				user,
				loginModel.Password,
				false,
				false
			);

			if (result.Succeeded)
			{
				_logger.LogInformation("User logged in");
			}
			else
			{
				return new LoginResponse
				{
					Errors = new[] { "Wrong password or email" },
					IsSuccess = false,
				};
			}

			if (!user.EmailConfirmed)
			{
				return new LoginResponse
				{
					Errors = new[] { "User is not active" },
					IsSuccess = false,
				};
			}

			var token = await GenerateJWTTokenWithUserClaimsAsync(user);

			var roles = new List<string>()
			{
				AccountRole.Buyer.ToString(),
				AccountRole.Seller.ToString()
			};

			token.Roles = roles;

			return new LoginResponse { IsSuccess = true, Result = token };
		}

		public async Task<LoginResponse> CheckEligibilityToLoginWithSelectedRole(string role)
		{
			if (role is null)
			{
				return new LoginResponse { Errors = new[] { "Role is null" }, IsSuccess = false, };
			}

			var roleExists = await _roleManager.RoleExistsAsync(role);

			if (!roleExists)
			{
				return new LoginResponse
				{
					Errors = new[] { "Specified role not found" },
					IsSuccess = false,
				};
			}

			if (role == AccountRole.Administrator.ToString())
			{
				return new LoginResponse
				{
					Errors = new[] { "Administrator role is not allowed" },
					IsSuccess = false,
				};
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
			);

			if (user is null)
			{
				return new LoginResponse
				{
					Errors = new[] { "User is not found or deleted" },
					IsSuccess = false,
				};
			}

			var userRoles = await _userRoleDbContextService.GetUnpaginatedListAsync(
				ur => ur.UserId == user.Id && !ur.IsDeleted
			);

			if (userRoles.Any())
			{
				foreach (var userRole in userRoles)
				{
					if (userRole.RoleId == (await _roleManager.FindByNameAsync(role))!.Id)
					{
						var token = await GenerateJWTTokenWithUserClaimsAsync(user, role);

						token.Role = role;
						token.UserId = user.Id;

						return new LoginResponse { IsSuccess = true, Result = token };
					}
				}
			}

			return new LoginResponse
			{
				Message = $"User has no {role} role, do you want to create one?"
			};
		}

		public async Task<LoginResponse> CreateNewUserWithNewRole(string role)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
			);

			if (user is null)
			{
				return new LoginResponse { IsSuccess = false, Message = "User not found" };
			}

			var roleExists = await _roleManager.RoleExistsAsync(role);

			if (!roleExists)
			{
				return new LoginResponse { IsSuccess = false, Message = "Role not found" };
			}

			if (role == AccountRole.Administrator.ToString())
			{
				return new LoginResponse
				{
					IsSuccess = false,
					Message = "Administrator role is not allowed"
				};
			}

			var userRoles = await _userManager.GetRolesAsync(user);

			if (userRoles == null)
			{
				return new LoginResponse { IsSuccess = false, Message = "User has no roles" };
			}

			if (userRoles.Any())
			{
				foreach (var userRole in userRoles)
				{
					if (userRole == role)
					{
						return new LoginResponse
						{
							IsSuccess = false,
							Message = $"User already has {role} account"
						};
					}
				}
			}

			var newUserRole = new UserRole
			{
				UserId = user.Id,
				RoleId = (await _roleManager.FindByNameAsync(role))!.Id,
				CreationDate = DateTime.UtcNow,
				IsDeleted = false
			};

			var result = await _userRoleDbContextService.AddAsync(newUserRole);

			if (result is not null)
			{
				var token = await GenerateJWTTokenWithUserClaimsAsync(user, role);

				token.Role = role;
				token.UserId = user.Id;

				return new LoginResponse
				{
					IsSuccess = true,
					Result = token,
					Message = $"Role '{role}' assigned to the user successfully"
				};
			}
			else
			{
				return new LoginResponse { IsSuccess = false, Message = "Failed to assign role" };
			}
		}

		/// <summary>
		/// Generation of JWT token with User claims including Email and role
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private async Task<TokenModel> GenerateJWTTokenWithUserClaimsAsync(
			User user,
			string role = null
		)
		{
			var claims = new List<Claim> { new(ClaimTypes.Email, user.Email!) };

			if (role is not null)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			else
			{
				var userRoles = await _userRoleDbContextService.GetUnpaginatedListAsync(
					ur => ur.UserId == user.Id && !ur.IsDeleted
				);

				if (userRoles.Any())
				{
					foreach (var userRole in userRoles)
					{
						var currentRole = await _roleManager.FindByIdAsync(
							userRole.RoleId.ToString()
						);

						claims.Add(new Claim(ClaimTypes.Role, currentRole!.Name!));
					}
				}
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettingsOptions.Key));

			var token = new JwtSecurityToken(
				issuer: _authSettingsOptions.Issuer,
				audience: _authSettingsOptions.Audience,
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

			return new TokenModel { AccessToken = tokenAsString, ExpireDate = token.ValidTo };
		}

		public async Task<ResetPasswordResponse> ForgetPasswordAsync(string email)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == email && !u.IsDeleted
			);

			if (user is null)
			{
				return new ResetPasswordResponse
				{
					IsSuccess = false,
					Message = "No user associated with email"
				};
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			var encodedToken = Encoding.UTF8.GetBytes(token);
			var validToken = WebEncoders.Base64UrlEncode(encodedToken);

			string url =
				$"{_appOptions.ClientApp}/auth/reset-password?email={email}&token={validToken}";

			await _emailService.SendEmailAsync(
				email,
				"Reset Password",
				"<h1>Follow the instructions to reset your password</h1>"
					+ $"<p>To reset your password <a href='{url}'>Click here</a></p>"
			);

			return new ResetPasswordResponse
			{
				IsSuccess = true,
				Message = "Reset password URL has been sent to the email successfully"
			};
		}

		public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel model)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == model.Email && !u.IsDeleted
			);

			if (user is null)
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

			var result = await _userManager.ResetPasswordAsync(
				user,
				normalToken,
				model.NewPassword
			);

			if (result.Succeeded)
				return new ResetPasswordResponse
				{
					Message = "Password has been reset successfully!",
					IsSuccess = true
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
			if (model is null)
				throw new NullReferenceException("Register Model is null")!;

			if (model.Password != model.ConfirmPassword)
				return new RegisterResponse
				{
					Message = "Confirm password doesn't match the password",
					IsSuccess = false,
				};

			var existingUser = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == model.Email && !u.IsDeleted
			);

			if (existingUser is not null)
				return new RegisterResponse
				{
					Message = "User with this email already exists",
					IsSuccess = false,
				};

			var newUsername = GenerateUniqueUserName();

			var user = new User
			{
				Email = model.Email,
				UserName = newUsername,
				IsDeleted = false,
				CreationDate = DateTime.UtcNow
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(
					user
				);

				var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
				var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

				var url =
					$"{_appOptions.Url}/api/auth/confirm-email?userid={user.Id}&token={validEmailToken}";

				await _emailService.SendEmailAsync(
					user.Email,
					"Confirm your email",
					$"<h1>Welcome to Auctionify</h1>"
						+ $"<p>Please confirm your email by <a href='{url}'>clicking here</a></p>"
				);

				return new RegisterResponse
				{
					Message = "User created successfully!",
					IsSuccess = true,
				};
			}

			return new RegisterResponse
			{
				Message = "User was not created",
				IsSuccess = false,
				Errors = result.Errors.Select(e => e.Description),
			};
		}

		private static string GenerateUniqueUserName()
		{
			return $"user_{Guid.NewGuid().ToString("N")}";
		}

		public async Task<RegisterResponse> ConfirmUserEmailAsync(string userId, string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return new RegisterResponse { IsSuccess = false, Message = "User not found", };

			var decodedToken = WebEncoders.Base64UrlDecode(token);
			string normalToken = Encoding.UTF8.GetString(decodedToken);

			var result = await _userManager.ConfirmEmailAsync(user, normalToken);

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

		public async Task<LoginResponse> AssignRoleToUserAsync(string role)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
			);

			if (user == null)
			{
				return new LoginResponse { IsSuccess = false, Message = "User not found" };
			}

			var userRoleList = await _userManager.GetRolesAsync(user);

			if (userRoleList.Any())
			{
				return new LoginResponse { IsSuccess = false, Message = "User already has a role" };
			}

			if (string.IsNullOrWhiteSpace(role))
			{
				return new LoginResponse
				{
					IsSuccess = false,
					Message = "Role name is not provided"
				};
			}

			var roleExists = await _roleManager.RoleExistsAsync(role);

			if (!roleExists)
			{
				return new LoginResponse { IsSuccess = false, Message = "Role not found" };
			}

			var newUserRole = new UserRole
			{
				UserId = user.Id,
				RoleId = (await _roleManager.FindByNameAsync(role))!.Id,
				CreationDate = DateTime.UtcNow,
				IsDeleted = false
			};

			var result = await _userRoleDbContextService.AddAsync(newUserRole);

			if (result is not null)
			{
				var token = await GenerateJWTTokenWithUserClaimsAsync(user, role);

				token.Role = role;
				token.UserId = user.Id;

				return new LoginResponse
				{
					IsSuccess = true,
					Result = token,
					Message = $"Role '{role}' assigned to the user successfully"
				};
			}
			else
			{
				return new LoginResponse { IsSuccess = false, Message = "Failed to assign role" };
			}
		}

		public async Task<LoginResponse> LoginUserWithGoogleAsync(Payload payload)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == payload.Email && !u.IsDeleted
			);

			if (user == null)
			{
				user = new User
				{
					Email = payload.Email,
					UserName = payload.Email,
					FirstName = payload.GivenName,
					LastName = payload.FamilyName,
					EmailConfirmed = true
				};

				var createdResult = await _userManager.CreateAsync(user);

				if (!createdResult.Succeeded)
				{
					return new LoginResponse
					{
						IsSuccess = false,
						Errors = createdResult.Errors.Select(e => e.Description)
					};
				}
			}

			var token = await GenerateJWTTokenWithUserClaimsAsync(user);

			token.UserId = user.Id;
			token.Roles = new List<string>()
			{
				AccountRole.Buyer.ToString(),
				AccountRole.Seller.ToString()
			};

			return new LoginResponse { IsSuccess = true, Result = token };
		}

		public async Task<ChangePasswordResponse> ChangeUserPasswordAsync(
			string email,
			ChangePasswordViewModel model
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == email && !u.IsDeleted
			);

			if (user is null)
			{
				return new ChangePasswordResponse
				{
					IsSuccess = false,
					Message = "No user associated with the provided email"
				};
			}

			var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
			if (!isOldPasswordValid)
			{
				return new ChangePasswordResponse
				{
					IsSuccess = false,
					Message = "Invalid old password"
				};
			}

			if (model.NewPassword != model.ConfirmNewPassword)
			{
				return new ChangePasswordResponse
				{
					IsSuccess = false,
					Message = "New password does not match its confirmation"
				};
			}

			var result = await _userManager.ChangePasswordAsync(
				user,
				model.OldPassword,
				model.NewPassword
			);

			if (result.Succeeded)
			{
				return new ChangePasswordResponse
				{
					IsSuccess = true,
					Message = "Password has been changed successfully!"
				};
			}

			return new ChangePasswordResponse
			{
				IsSuccess = false,
				Message = "Something went wrong",
				Errors = result.Errors.Select(e => e.Description)
			};
		}
	}
}
