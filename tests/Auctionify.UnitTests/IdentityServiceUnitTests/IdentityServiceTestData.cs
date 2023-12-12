using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Common.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Auctionify.UnitTests.IdentityServiceUnitTests
{
	public static class IdentityServiceTestData
	{
		public static IEnumerable<object[]> GetLoginUserAsyncTestData()
		{
			var users = new List<User>
			{
				new User
				{
					Id = 1,
					UserName = "test@email.com",
					NormalizedUserName = "TEST@EMAIL.COM",
					Email = "test@email.com",
					NormalizedEmail = "TEST@EMAIL.COM",
					EmailConfirmed = true, // email is confirmed
					PasswordHash = "testpasswordhash",
					SecurityStamp = "testsecuritystamp",
					ConcurrencyStamp = "testconcurrencystamp",
					PhoneNumber = "123456789",
					FirstName = "TestFirstName",
					LastName = "TestLastName",
				},
				new User
				{
					Id = 1,
					UserName = "test@email.com",
					NormalizedUserName = "TEST@EMAIL.COM",
					Email = "test@email.com",
					NormalizedEmail = "TEST@EMAIL.COM",
					EmailConfirmed = false, // email is not confirmed
					PasswordHash = "testpasswordhash",
					SecurityStamp = "testsecuritystamp",
					ConcurrencyStamp = "testconcurrencystamp",
					PhoneNumber = "123456789",
					FirstName = "TestFirstName",
					LastName = "TestLastName",
				}
			};

			var authSettingsOptions = new AuthSettingsOptions
			{
				Key = "This is a sample test key",
				Issuer = "https://testlocalhost:1234",
				Audience = "https://testlocalhost:1234",
				AccessTokenExpirationInMinutes = 10,
				RefreshTokenExpirationInDays = 60
			};

			var loginViewModels = new List<LoginViewModel>
			{
				new LoginViewModel { Email = "test@email.com", Password = "testpassword" }, // correct credentials
				new LoginViewModel { Email = "nosuchuser@email.com", Password = "testpassword" }, // no such user
				new LoginViewModel { Email = "test@email.com", Password = "wrongtestpassword" } // wrong password
			};

			yield return new object[] { users, authSettingsOptions, loginViewModels };
		}

		public static IEnumerable<object[]> GetResetPasswordAsyncTestData()
		{
			var resetPasswordViewModels = new List<ResetPasswordViewModel>
			{
				new ResetPasswordViewModel
				{
					Token = "simpletesttoken",
					Email = "test@email.com",
					NewPassword = "testpassword",
					ConfirmPassword = "testpassword" // password matches its confirmation
                },
				new ResetPasswordViewModel
				{
					Token = "simpletesttoken",
					Email = "test@email.com",
					NewPassword = "testpassword",
					ConfirmPassword = "wrongtestpassword" // password does not match its confirmation
                }
			};

			yield return new object[] { resetPasswordViewModels };
		}

		public static IEnumerable<object[]> GetRegisterUserAsyncTestData()
		{
			var registerViewModels = new List<RegisterViewModel>
			{
				new RegisterViewModel
				{
					Email = "test@email.com",
					Password = "testpassword",
					ConfirmPassword = "testpassword"
				},
				new RegisterViewModel
				{
					Email = "test@email.com",
					Password = "testpassword",
					ConfirmPassword = "wrongconfirmtestpassword"
				}
			};

			yield return new object[] { registerViewModels };
		}

		public static IEnumerable<object[]> GetConfirmUserEmailAsyncTestData()
		{
			var userId = "1";
			var token = "simpletesttoken";

			yield return new object[] { userId, token };
		}

		public static IEnumerable<object[]> GetLoginUserWithGoogleAsyncTestData()
		{
			var payload = new Payload
			{
				Email = "test@gmail.com",
				GivenName = "testFirstName",
				FamilyName = "testLastName"
			};

			var user = new User
			{
				Email = "test@gmail.com",
				UserName = "test@gmail.com",
				FirstName = "testFirstName",
				LastName = "testLastName",
				EmailConfirmed = true
			};

			var authSettingsOptions = new AuthSettingsOptions
			{
				Key = "This is a sample test key",
				Issuer = "https://testlocalhost:1234",
				Audience = "https://testlocalhost:1234",
				AccessTokenExpirationInMinutes = 10,
				RefreshTokenExpirationInDays = 60
			};

			yield return new object[] { payload, user, authSettingsOptions };
		}
	}
}
