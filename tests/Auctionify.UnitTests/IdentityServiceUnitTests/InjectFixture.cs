using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Identity;
using Auctionify.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.UnitTests.IdentityServiceUnitTests
{
	public class InjectFixture : IDisposable
	{
		#region Initialization

		public readonly UserManager<User> UserManager;
		public readonly SignInManager<User> SignInManager;
		public readonly IIdentityService IdentityService;
		public readonly ApplicationDbContext DbContext;


		#endregion

		#region Tests

		#endregion

		#region Deinitialization
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
