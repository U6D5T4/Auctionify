using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.IdentityServiceUnitTests
{
	public class FakeUserManager : UserManager<User>
	{
		//public FakeUserManager(IUserStore<User> store,
		//	IOptions<IdentityOptions> optionsAccessor,
		//	IPasswordHasher<User> passwordHasher,
		//	IEnumerable<IUserValidator<User>> userValidators,
		//	IEnumerable<IPasswordValidator<User>> passwordValidators,
		//	ILookupNormalizer keyNormalizer,
		//	IdentityErrorDescriber errors,
		//	IServiceProvider services,
		//	ILogger<UserManager<User>> logger)
		//	: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		//{
		//}

		public FakeUserManager()
			: base(
				new Mock<IUserStore<User>>().Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<IPasswordHasher<User>>().Object,
				new Mock<IEnumerable<IUserValidator<User>>>().Object,
				new Mock<IEnumerable<IPasswordValidator<User>>>().Object,
				new Mock<ILookupNormalizer>().Object,
				new Mock<IdentityErrorDescriber>().Object,
				new Mock<IServiceProvider>().Object,
				new Mock<ILogger<UserManager<User>>>().Object
			)
		{ }
	}
}
