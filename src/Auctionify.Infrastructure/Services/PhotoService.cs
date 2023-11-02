using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Auctionify.Infrastructure.Services
{
	public class PhotoService : IPhotoService
	{
		private readonly IFileRepository _fileRepository;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

		public PhotoService(IFileRepository fileRepository,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
			)
		{
			_fileRepository = fileRepository;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
		}

		public async Task<string?> GetMainPhotoUrlAsync(int lotId, CancellationToken cancellationToken)
		{
			var photo = await _fileRepository.GetAsync(
				predicate: x =>
					x.LotId == lotId
					&& x.Path.Contains(_azureBlobStorageOptions.PhotosFolderName),
				cancellationToken: cancellationToken
			);

			if (photo != null)
			{
				return _blobService.GetBlobUrl(photo.Path, photo.FileName);
			}

			return null;
		}
	}
}
