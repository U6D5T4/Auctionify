using Auctionify.Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Auctionify.Infrastructure.Services
{
	public class BlobService : IBlobService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public BlobService(BlobServiceClient blobServiceClient)
		{
			_blobServiceClient = blobServiceClient;
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
		public string GetBlobUrl(string filePath, string fileName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient($"{filePath}/{fileName}");
			var url = blobClient.Uri.AbsoluteUri;

			return url;
		}

		public async Task DeleteFileBlobAsync(string filePath, string fileName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("auctionify-files");
			var blobClient = containerClient.GetBlobClient($"{filePath}/{fileName}");

			await blobClient.DeleteIfExistsAsync();
		}
	}
}
