using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Common.Options;
using Auctionify.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using User = Auctionify.Core.Entities.User;

namespace Auctionify.UnitTests.IdentityServiceUnitTests
{
	public class IdentityServiceUnitTests : IDisposable
	{
		#region Initialization

		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly Mock<SignInManager<User>> _signInManagerMock;
		private readonly Mock<ILogger<IdentityService>> _loggerMock;
		private readonly Mock<IOptions<AuthSettingsOptions>> _authSettingsOptionsMock;
		private readonly Mock<IOptions<AppOptions>> _appOptionsMock;
		private readonly Mock<IEmailService> _emailServiceMock;
		private readonly Mock<RoleManager<Role>> _roleManagerMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;

		public IdentityServiceUnitTests()
		{
			_userManagerMock = new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_signInManagerMock = new Mock<SignInManager<User>>(
				_userManagerMock.Object,
				Mock.Of<IHttpContextAccessor>(),
				Mock.Of<IUserClaimsPrincipalFactory<User>>(),
				Mock.Of<IOptions<IdentityOptions>>(),
				Mock.Of<ILogger<SignInManager<User>>>(),
				Mock.Of<IAuthenticationSchemeProvider>(),
				Mock.Of<IUserConfirmation<User>>()
			);
			_loggerMock = new Mock<ILogger<IdentityService>>();
			_authSettingsOptionsMock = new Mock<IOptions<AuthSettingsOptions>>();
			_appOptionsMock = new Mock<IOptions<AppOptions>>();
			_emailServiceMock = new Mock<IEmailService>();
			_roleManagerMock = new Mock<RoleManager<Role>>(
				Mock.Of<IRoleStore<Role>>(),
				null,
				null,
				null,
				null
			);
		}

		#endregion

		#region Tests

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserAsync_WithCorrectLoginCredentialsAndWhenUserExists_ReturnsSucceededLoginResponse(
			List<User> users,
			AuthSettingsOptions authSettingsOptions,
			List<LoginViewModel> loginViewModels
		)
		{
			// Arrange
			var user = users[0];
			var userModel = loginViewModels[0];

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			_signInManagerMock
				.Setup(m => m.PasswordSignInAsync(user, userModel.Password, false, false))
				.ReturnsAsync(SignInResult.Success);

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserAsync(userModel);

			// Assert
			result.Should().NotBeNull();
			result.IsSuccess.Should().BeTrue();
			result.Result.Should().NotBeNull();
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserAsync_WithCorrectLoginCredentialsAndWhenUserExistsButEmailIsNotConfirmed_ReturnsFalseLoginResponse(
			List<User> users,
			AuthSettingsOptions authSettingsOptions,
			List<LoginViewModel> loginViewModels
		)
		{
			// Arrange
			var user = users[1];
			var userModel = loginViewModels[0];

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			_signInManagerMock
				.Setup(m => m.PasswordSignInAsync(user, userModel.Password, false, false))
				.ReturnsAsync(SignInResult.Success);

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserAsync(userModel);

			// Assert
			result.Errors.FirstOrDefault().Should().Be("User is not active");
			result.Should().NotBeNull();
			result.IsSuccess.Should().BeFalse();
			result.Result.Should().BeNull(); // null because no token is generated, since user is not active
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserAsync_WhenUserDoesNotExist_ReturnsFalseLoginResponse(
			List<User> users,
			AuthSettingsOptions authSettingsOptions,
			List<LoginViewModel> loginViewModels
		)
		{
			// Arrange
			var user = users[0];
			var userModel = loginViewModels[1];

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			_signInManagerMock
				.Setup(m => m.PasswordSignInAsync(user, userModel.Password, false, false))
				.ReturnsAsync(SignInResult.Success);

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserAsync(userModel);

			// Assert
			result.Errors.FirstOrDefault().Should().Be("User is not found or deleted");
			result.Should().NotBeNull();
			result.IsSuccess.Should().BeFalse();
			result.Result.Should().BeNull();
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserAsync_WithCorrectLoginEmailAndWhenUserExistsButPasswordIsWrong_ReturnsFalseLoginResponse(
			List<User> users,
			AuthSettingsOptions authSettingsOptions,
			List<LoginViewModel> loginViewModels
		)
		{
			// Arrange
			var user = users[0];
			var userModel = loginViewModels[2];

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			_signInManagerMock
				.Setup(m => m.PasswordSignInAsync(user, userModel.Password, false, false))
				.ReturnsAsync(SignInResult.Failed);

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserAsync(userModel);

			// Assert
			result.Errors.FirstOrDefault().Should().Be("Wrong password or email");
			result.Should().NotBeNull();
			result.IsSuccess.Should().BeFalse();
			result.Result.Should().BeNull();
		}

		[Fact]
		public async Task LoginUserAsync_WhenUserModelIsNull_ReturnsFalseLoginResponse()
		{
			// Arrange
			LoginViewModel? userModel = null;

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserAsync(userModel!);

			// Assert
			result.Errors.FirstOrDefault().Should().Be("User data is empty");
			result.Should().NotBeNull();
			result.IsSuccess.Should().BeFalse();
			result.Result.Should().BeNull();
		}

		[Fact]
		public async Task ForgetPasswordAsync_WhenUserDoesNotExist_ReturnsFalseResetPasswordResponse()
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ForgetPasswordAsync(It.IsAny<string>());

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("No user associated with email");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ForgetPasswordAsync_WhenUserExists_SendsEmailToUserAndReturnsTrueResetPasswordResponse(
			List<User> users,
			AuthSettingsOptions authSettingsOptions, // not used in this test
			List<LoginViewModel> loginViewModels
		)
		{
			// Arrange
			var email = loginViewModels[0].Email;
			var token = "simpletesttoken";
			var encodedToken = Encoding.UTF8.GetBytes(token);
			var validToken = WebEncoders.Base64UrlEncode(encodedToken);
			var user = users[0];

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GeneratePasswordResetTokenAsync(user))
				.ReturnsAsync(token);

			_appOptionsMock
				.Setup(options => options.Value)
				.Returns(new AppOptions { ClientApp = "https://testlocalhost:1234" });

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ForgetPasswordAsync(email);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result
				.Message.Should()
				.Be("Reset password URL has been sent to the email successfully");

			_emailServiceMock.Verify(
				m =>
					m.SendEmailAsync(
						email,
						"Reset Password",
						"<h1>Follow the instructions to reset your password</h1>"
							+ "<p>To reset your password "
							+ $"<a href='https://testlocalhost:1234/auth/reset-password?email={email}&token={validToken}'>"
							+ "Click here</a></p>"
					),
				Times.Once
			);
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetResetPasswordAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ResetPasswordAsync_WhenUserDoesNotExistOrWhenUserModelIsNull_ReturnsFalseResetPasswordResponse(
			List<ResetPasswordViewModel> resetPasswordViewModels
		)
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ResetPasswordAsync(resetPasswordViewModels[2]);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("No user associated with email");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetResetPasswordAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ResetPasswordAsync_WhenUserExistsButPasswordDoesNotMatchItsConfirmation_ReturnsFalseResetPasswordResponse(
			List<ResetPasswordViewModel> resetPasswordViewModels
		)
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock
				.Setup(m => m.UserEmail)
				.Returns(resetPasswordViewModels[1].Email);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ResetPasswordAsync(resetPasswordViewModels[1]);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("Password does not match its confirmation");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetResetPasswordAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ResetPasswordAsync_WhenUserExistsAndPasswordMatchesItsConfirmation_ReturnsTrueResetPasswordResponse(
			List<ResetPasswordViewModel> resetPasswordViewModels
		)
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock
				.Setup(m => m.UserEmail)
				.Returns(resetPasswordViewModels[0].Email);

			_userManagerMock
				.Setup(
					m =>
						m.ResetPasswordAsync(
							It.IsAny<User>(),
							It.IsAny<string>(),
							It.IsAny<string>()
						)
				)
				.ReturnsAsync(IdentityResult.Success);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ResetPasswordAsync(resetPasswordViewModels[0]);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Message.Should().Be("Password has been reset successfully!");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetResetPasswordAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ResetPasswordAsync_WhenResultSucceededIsFalse_ReturnsFalseResetPasswordResponse(
			List<ResetPasswordViewModel> resetPasswordViewModels
		)
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock
				.Setup(m => m.UserEmail)
				.Returns(resetPasswordViewModels[0].Email);

			_userManagerMock
				.Setup(
					m =>
						m.ResetPasswordAsync(
							It.IsAny<User>(),
							It.IsAny<string>(),
							It.IsAny<string>()
						)
				)
				.ReturnsAsync(
					IdentityResult.Failed(new IdentityError { Description = "testerror" })
				);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ResetPasswordAsync(resetPasswordViewModels[0]);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("Something went wrong");
			result.Errors.FirstOrDefault().Should().Be("testerror");
		}

		[Fact]
		public async Task RegisterUserAsync_WhenRegisterModelIsNull_ThrowsNullReferenceException()
		{
			// Arrange
			RegisterViewModel? registerModel = null;

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			Func<Task> act = async () => await sut.RegisterUserAsync(registerModel!);

			// Assert
			await act.Should()
				.ThrowExactlyAsync<NullReferenceException>()
				.WithMessage("Register Model is null");
			await act.Should().NotThrowAsync<ArgumentException>();
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetRegisterUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task RegisterUserAsync_WhenPasswordDoesNotMatchItsConfirmation_ReturnsFalseRegisterResponse(
			List<RegisterViewModel> registerViewModels
		)
		{
			// Arrange
			var registerModel = registerViewModels[1];

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.RegisterUserAsync(registerModel);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("Confirm password doesn't match the password");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetRegisterUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task RegisterUserAsync_WhenUserIsCreatedSuccessfully_ReturnsTrueRegisterResponse(
			List<RegisterViewModel> registerViewModels
		)
		{
			// Arrange
			var token = "simpletesttoken";
			var encodedToken = Encoding.UTF8.GetBytes(token);
			var validToken = WebEncoders.Base64UrlEncode(encodedToken);
			var registerModel = registerViewModels[0];

			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

			_userManagerMock
				.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
				.ReturnsAsync(token);

			_appOptionsMock
				.Setup(options => options.Value)
				.Returns(new AppOptions { Url = "https://testlocalhost:1234" });

			_emailServiceMock
				.Setup(
					m =>
						m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
				)
				.Returns(Task.CompletedTask);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.RegisterUserAsync(registerModel);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Message.Should().Be("User created successfully!");
			_emailServiceMock.Verify(
				m =>
					m.SendEmailAsync(
						registerModel.Email,
						"Confirm your email",
						$"<h1>Welcome to Auctionify</h1>"
							+ $"<p>Please confirm your email by "
							+ $"<a href='https://testlocalhost:1234/api/auth/confirm-email?userid=0&token={validToken}'>"
							+ $"clicking here</a></p>"
					),
				Times.Once
			);
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetRegisterUserAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task RegisterUserAsync_WhenUserIsNotCreatedSuccessfully_ReturnsFalseRegisterResponse(
			List<RegisterViewModel> registerViewModels
		)
		{
			// Arrange
			var registerModel = registerViewModels[0];

			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(
					IdentityResult.Failed(new IdentityError { Description = "testerror" })
				);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.RegisterUserAsync(registerModel);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("User was not created");
			result.Errors.FirstOrDefault().Should().Be("testerror");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetConfirmUserEmailAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ConfirmUserEmailAsync_WhenUserIdIsInvalidOrUserIsNull_ReturnsFalseRegisterResponse(
			string userId,
			string token
		)
		{
			// Arrange
			_userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((User?)null);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ConfirmUserEmailAsync(userId, token);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("User not found");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetConfirmUserEmailAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ConfirmUserEmailAsync_WhenUserIdIsValidAndUserIsNotNullAndResultIsSucceeded_ReturnsTrueRegisterResponse(
			string userId,
			string token
		)
		{
			// Arrange
			_userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(new User());

			_userManagerMock
				.Setup(m => m.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ConfirmUserEmailAsync(userId, token);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Message.Should().Be("Email confirmed successfully!");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetConfirmUserEmailAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task ConfirmUserEmailAsync_WhenUserIsNotNullAndResultIsNotSucceeded_ReturnsFalseRegisterResponse(
			string userId,
			string token
		)
		{
			// Arrange
			_userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(new User());
			_userManagerMock
				.Setup(m => m.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(
					IdentityResult.Failed(new IdentityError { Description = "testerror" })
				);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ConfirmUserEmailAsync(userId, token);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("Email did not confirm");
			result.Errors.FirstOrDefault().Should().Be("testerror");
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserWithGoogleAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserWithGoogleAsync_WhenUserAndRoleAreNotNull_ReturnsTokenWithRole(
			Payload payload,
			User user,
			AuthSettingsOptions authSettingsOptions
		)
		{
			// Arrange
			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock
				.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserWithGoogleAsync(payload);

			// Assert
			result.Result.Should().NotBeNull();
			result.IsSuccess.Should().BeTrue();
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserWithGoogleAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserWithGoogleAsync_WhenUserIsNotNullAndRoleIsNull_ReturnsTokenWithEmptyRole(
			Payload payload,
			User user,
			AuthSettingsOptions authSettingsOptions
		)
		{
			// Arrange
			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

			_authSettingsOptionsMock.Setup(options => options.Value).Returns(authSettingsOptions);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserWithGoogleAsync(payload);

			// Assert
			result.Result.Should().NotBeNull();
			result.IsSuccess.Should().BeTrue();
		}

		[Theory]
		[MemberData(
			nameof(IdentityServiceTestData.GetLoginUserWithGoogleAsyncTestData),
			MemberType = typeof(IdentityServiceTestData)
		)]
		public async Task LoginUserWithGoogleAsync_WhenUserIsNullAndResultIsNotSucceeded_ReturnsFalseLoginResponse(
			Payload payload,
			User user, // not used in this test
			AuthSettingsOptions authSettingsOptions // not used in this test
		)
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CreateAsync(It.IsAny<User>()))
				.ReturnsAsync(
					IdentityResult.Failed(new IdentityError { Description = "testerror" })
				);

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object,
				_roleManagerMock.Object,
				_authSettingsOptionsMock.Object,
				_appOptionsMock.Object,
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.LoginUserWithGoogleAsync(payload);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Errors.FirstOrDefault().Should().Be("testerror");
		}

		[Fact]
		public async Task ChangeUserPasswordAsync_WhenUserIsNull_ReturnsFalseChangePasswordResponse()
		{
			// Arrange
			var mock = new List<User> { EntitiesSeeding.GetUsers()[0] }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CreateAsync(It.IsAny<User>()))
				.ReturnsAsync(
					IdentityResult.Failed(new IdentityError { Description = "testerror" })
				);

			var changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = "testoldpassword",
				NewPassword = "testnewpassword",
				ConfirmNewPassword = "testnewpassword"
			};

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object, // not used in this test
				_roleManagerMock.Object, // not used in this test
				_authSettingsOptionsMock.Object, // not used in this test
				_appOptionsMock.Object, // not used in this test
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ChangeUserPasswordAsync(
				It.IsAny<string>(),
				changePasswordViewModel
			);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("No user associated with the provided email");
		}

		[Fact]
		public async Task ChangeUserPasswordAsync_WhenOldPasswordIsInvalid_ReturnsFalseChangePasswordResponse()
		{
			// Arrange
			var mock = new List<User> { new User() }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(false);

			var changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = "testoldpassword",
				NewPassword = "testnewpassword",
				ConfirmNewPassword = "testnewpassword"
			};

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object, // not used in this test
				_roleManagerMock.Object, // not used in this test
				_authSettingsOptionsMock.Object, // not used in this test
				_appOptionsMock.Object, // not used in this test
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ChangeUserPasswordAsync(
				It.IsAny<string>(),
				changePasswordViewModel
			);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("Invalid old password");
		}

		[Fact]
		public async Task ChangeUserPasswordAsync_WhenNewPasswordDoesNotMatchItsConfirmation_ReturnsFalseChangePasswordResponse()
		{
			// Arrange
			var mock = new List<User> { new User() }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(true);

			var changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = "testoldpassword",
				NewPassword = "testnewpassword",
				ConfirmNewPassword = "testnewpassword1"
			};

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object, // not used in this test
				_roleManagerMock.Object, // not used in this test
				_authSettingsOptionsMock.Object, // not used in this test
				_appOptionsMock.Object, // not used in this test
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ChangeUserPasswordAsync(
				It.IsAny<string>(),
				changePasswordViewModel
			);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("New password does not match its confirmation");
		}

		[Fact]
		public async Task ChangeUserPasswordAsync_WhenResultIsSucceeded_ReturnsTrueChangePasswordResponse()
		{
			// Arrange
			var mock = new List<User> { new User() }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(It.IsAny<string>());

			_userManagerMock
				.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
				.ReturnsAsync(true);
			_userManagerMock
				.Setup(
					m =>
						m.ChangePasswordAsync(
							It.IsAny<User>(),
							It.IsAny<string>(),
							It.IsAny<string>()
						)
				)
				.ReturnsAsync(IdentityResult.Success);

			var changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = "testoldpassword",
				NewPassword = "testnewpassword",
				ConfirmNewPassword = "testnewpassword"
			};

			var sut = new IdentityService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_loggerMock.Object,
				_emailServiceMock.Object, // not used in this test
				_roleManagerMock.Object, // not used in this test
				_authSettingsOptionsMock.Object, // not used in this test
				_appOptionsMock.Object, // not used in this test
				_currentUserServiceMock.Object
			);

			// Act
			var result = await sut.ChangeUserPasswordAsync(
				It.IsAny<string>(),
				changePasswordViewModel
			);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Message.Should().Be("Password has been changed successfully!");
		}

		#endregion

		#region Deinitialization

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_userManagerMock.Reset();
				_signInManagerMock.Reset();
				_loggerMock.Reset();
				_authSettingsOptionsMock.Reset();
				_appOptionsMock.Reset();
				_emailServiceMock.Reset();
				_roleManagerMock.Reset();
				_currentUserServiceMock.Reset();
			}
		}

		#endregion
	}
}
