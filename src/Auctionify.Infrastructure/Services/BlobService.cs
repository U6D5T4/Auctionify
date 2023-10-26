using Auctionify.Application.Common.Extensions;
using Auctionify.Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
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

		public async Task<Application.Common.Models.Blob.BlobInfo> GetBlobAsync(string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(blobName);
			var blobDownloadInfo = await blobClient.DownloadAsync();

			return new Application.Common.Models.Blob.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
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
		public async Task UploadFileBlobAsync(IFormFile file, string filePath)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient($"{filePath}/{file.FileName}");

			if (await blobClient.ExistsAsync())
			{
				throw new InvalidOperationException($"File '{file.FileName}' already exists in the specified path.");
			}

			var blobUploadOptions = new BlobUploadOptions
			{
				HttpHeaders = new BlobHttpHeaders
				{
					ContentType = file.ContentType
				}
			};

			await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
		}

		public async Task UploadContentBlobAsync(string content, string fileName)
		{
			// Generate a GUID and append it to the fileName before creating the blobClient
			string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);

			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(uniqueFileName);
			var bytes = Encoding.UTF8.GetBytes(content);

			await using var memoryStream = new MemoryStream(bytes);

			await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders
			{ ContentType = fileName.GetContentType() });
		}

		public async Task DeleteBlobAsync(string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient(blobName);

			await blobClient.DeleteIfExistsAsync();
		}

		public async Task UploadFilesBlobAsync(IList<IFormFile> files, string folderName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");

			foreach (var file in files)
			{
				var blobClient = containerClient.GetBlobClient($"{folderName}/{Guid.NewGuid()}");

				var blobUploadOptions = new BlobUploadOptions
				{
					HttpHeaders = new BlobHttpHeaders
					{
						ContentType = file.ContentType
					},
					Metadata = new Dictionary<string, string>
					{
						{ "originalFileName", file.FileName }
					}
				};

				await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
			}
		}


		public async Task<IEnumerable<string>> ListBlobsInFolderAsync(string folderName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var items = new List<string>();

			await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: folderName))
			{
				items.Add(blobItem.Name);
			}

			return items;
		}

		public async Task<Application.Common.Models.Blob.BlobInfo> GetBlobInFolderAsync(string folderName, string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient($"{folderName}/{blobName}");
			var blobDownloadInfo = await blobClient.DownloadAsync();

			return new Application.Common.Models.Blob.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
		}

		// delete the entire folder
		public async Task DeleteFolderAsync(string folderName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");

			await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: folderName))
			{
				await containerClient.DeleteBlobAsync(blobItem.Name);
			}
		}
	}
}
