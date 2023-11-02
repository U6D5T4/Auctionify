using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.IdentityServiceUnitTests
{
	public class FakeSignInManager : SignInManager<User>
	{
		//public FakeSignInManager(UserManager<User> userManager,
		//	IHttpContextAccessor contextAccessor,
		//	IUserClaimsPrincipalFactory<User> claimsFactory,
		//	IOptions<IdentityOptions> optionsAccessor,
		//	ILogger<SignInManager<User>> logger,
		//	IAuthenticationSchemeProvider schemes,
		//	IUserConfirmation<User> confirmation)
		//	: base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
		//{
		//}

		public FakeSignInManager()
			: base(
				new Mock<FakeUserManager>().Object,
				new Mock<IHttpContextAccessor>().Object,
				new Mock<IUserClaimsPrincipalFactory<User>>().Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<ILogger<SignInManager<User>>>().Object,
				new Mock<IAuthenticationSchemeProvider>().Object,
				new Mock<IUserConfirmation<User>>().Object
			)
		{ }
	}
}
