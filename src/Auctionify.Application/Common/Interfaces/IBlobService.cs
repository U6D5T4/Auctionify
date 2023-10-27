using Auctionify.Application.Common.Models.Blob;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace Auctionify.Application.Common.Interfaces
{
    public interface IBlobService
	{
		public Task<BlobInfo> GetBlobAsync(string blobName);
		public Task<IEnumerable<string>> ListBlobsAsync();
		public Task UploadFileBlobAsync(IFormFile file, string filePath);
		public string GetBlobUrl(string filePath, string fileName);
		public Task DeleteFileBlobAsync(string filePath, string fileName);



		public Task <BlobInfo>DownloadFileBlobAsync(string filePath, string fileName);

		public Task UploadContentBlobAsync(string content, string fileName);
		public Task DeleteBlobAsync(string blobName);
		public Task UploadFilesBlobAsync(IList<IFormFile> files, string folderName);
		public Task<IEnumerable<string>> ListBlobsInFolderAsync(string folderName);
		public Task<BlobInfo> GetBlobInFolderAsync(string folderName, string blobName);
		public Task DeleteFolderAsync(string folderName);
	}
}
