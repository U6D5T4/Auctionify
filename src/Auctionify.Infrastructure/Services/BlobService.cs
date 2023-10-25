using Auctionify.Application.Common.Extensions;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Blob;
using Azure.Storage.Blobs;
using System.Text;

namespace Auctionify.Infrastructure.Services
{
	public class BlobService : IBlobService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public BlobService(BlobServiceClient blobServiceClient)
		{
			_blobServiceClient = blobServiceClient;
		}

		public async Task<BlobInfo> GetBlobAsync(string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(blobName);
			var blobDownloadInfo = await blobClient.DownloadAsync();

			return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
		}

		public async Task<IEnumerable<string>> ListBlobsAsync()
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var items = new List<string>();

			await foreach (var blobItem in containerClient.GetBlobsAsync())
			{
				items.Add(blobItem.Name);
			}

			return items;
		}
		public async Task UploadFileBlobAsync(string filePath, string fileName)
		{
			// Generate a GUID and append it to the fileName before creating the blobClient
			string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);

			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(uniqueFileName);

			await blobClient.UploadAsync(filePath, new Azure.Storage.Blobs.Models.BlobHttpHeaders
			{ ContentType = filePath.GetContentType() });
		}

		public async Task UploadContentBlobAsync(string content, string fileName)
		{
			// Generate a GUID and append it to the fileName before creating the blobClient
			string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);

			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(uniqueFileName);
			var bytes = Encoding.UTF8.GetBytes(content);

			await using var memoryStream = new MemoryStream(bytes);

			await blobClient.UploadAsync(memoryStream, new Azure.Storage.Blobs.Models.BlobHttpHeaders
			{ ContentType = fileName.GetContentType() });
		}


		public async Task DeleteBlobAsync(string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(blobName);

			await blobClient.DeleteIfExistsAsync();
		}
	}
}
