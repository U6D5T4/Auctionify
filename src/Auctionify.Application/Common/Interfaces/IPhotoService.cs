namespace Auctionify.Application.Common.Interfaces
{
	public interface IPhotoService
	{
		public Task<string?> GetMainPhotoUrlAsync(int lotId, CancellationToken cancellationToken);
	}
}
