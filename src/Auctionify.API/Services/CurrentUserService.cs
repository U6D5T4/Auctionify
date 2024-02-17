using Auctionify.Application.Common.Interfaces;
using System.Security.Claims;

namespace Auctionify.API.Services
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public string? UserEmail =>
			_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

		public string? UserRole =>
			_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
	}
}
