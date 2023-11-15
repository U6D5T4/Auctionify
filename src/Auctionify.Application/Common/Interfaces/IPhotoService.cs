namespace Auctionify.Application.Common.Interfaces
{
	public interface IPhotoService
	{
		Task<string?> GetMainPhotoUrlAsync(int lotId, CancellationToken cancellationToken);
	}
}
