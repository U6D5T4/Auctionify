namespace Auctionify.Application.Common.Interfaces
{
	public interface ICurrentUserService
	{
		string? UserEmail { get; }

		string? UserRole { get; }
	}
}
