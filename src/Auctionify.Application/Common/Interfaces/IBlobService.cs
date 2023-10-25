using Auctionify.Application.Common.Models.Blob;

namespace Auctionify.Application.Common.Interfaces
{
    public interface IBlobService
	{
		public Task<BlobInfo> GetBlobAsync(string blobName);
		public Task<IEnumerable<string>> ListBlobsAsync();
		public Task UploadFileBlobAsync(string filePath, string fileName);
		public Task UploadContentBlobAsync(string content, string fileName);
		public Task DeleteBlobAsync(string blobName);
	}
}
