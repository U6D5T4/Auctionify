using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Common.Interfaces
{
	public interface IBlobService
	{
		Task UploadFileBlobAsync(IFormFile file, string filePath);
		string GetBlobUrl(string filePath, string fileName);
		Task DeleteFileBlobAsync(string filePath, string fileName);
	}
}
