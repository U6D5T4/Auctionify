using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Auctionify.Infrastructure.Services
{
	public class BlobService : IBlobService
	{
		private readonly BlobServiceClient _blobServiceClient;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

		public BlobService(
			BlobServiceClient blobServiceClient,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
		)
		{
			_blobServiceClient = blobServiceClient;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
		}

		public async Task UploadFileBlobAsync(
			IFormFile file,
			string filePath,
			string guid = null
		)
		{
			string fileName =
				guid != null
					? $"{Path.GetFileNameWithoutExtension(file.FileName)}_{guid}{Path.GetExtension(file.FileName)}"
					: file.FileName;

			var containerClient = _blobServiceClient.GetBlobContainerClient(
				_azureBlobStorageOptions.ContainerName
			);

			var blobClient = containerClient.GetBlobClient($"{filePath}/{fileName}");

			if (await blobClient.ExistsAsync())
			{
				throw new InvalidOperationException(
					$"File '{fileName}' already exists in the specified path."
				);
			}

			var blobUploadOptions = new BlobUploadOptions
			{
				HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
			};

			await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
		}

		public string GetBlobUrl(string filePath, string fileName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(
				_azureBlobStorageOptions.ContainerName
			);

			var blobClient = containerClient.GetBlobClient($"{filePath}/{fileName}");

			var url = blobClient.Uri.AbsoluteUri;

			return url;
		}

		public async Task DeleteFileBlobAsync(string filePath, string fileName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(
				_azureBlobStorageOptions.ContainerName
			);

			var blobClient = containerClient.GetBlobClient($"{filePath}/{fileName}");

			await blobClient.DeleteIfExistsAsync();
		}
	}
}
