using Auctionify.Application.Common.Models.Blob;
using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Common.Interfaces
{
    public interface IBlobService
	{
		public Task UploadFileBlobAsync(IFormFile file, string filePath);
		public string GetBlobUrl(string filePath, string fileName);
		public Task DeleteFileBlobAsync(string filePath, string fileName);
	}
}
