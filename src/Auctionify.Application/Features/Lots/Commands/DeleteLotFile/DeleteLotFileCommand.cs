using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Commands.DeleteLotFile
{
	public class DeleteLotFileCommand : IRequest<DeletedLotFileResponse>
	{
		public int LotId { get; set; }
		public List<string> FileUrl { get; set; }

		public class DeleteLotFileCommandHandler
			: IRequestHandler<DeleteLotFileCommand, DeletedLotFileResponse>
		{
			private readonly IMapper _mapper;
			private readonly IFileRepository _fileRepository;
			private readonly IBlobService _blobService;
			private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

			public DeleteLotFileCommandHandler(
				IMapper mapper,
				IFileRepository fileRepository,
				IBlobService blobService,
				IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
			)
			{
				_mapper = mapper;
				_fileRepository = fileRepository;
				_blobService = blobService;
				_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			}

			public async Task<DeletedLotFileResponse> Handle(
				DeleteLotFileCommand request,
				CancellationToken cancellationToken
			)
			{
				var photosUrl = request.FileUrl
					.Where(
						x =>
							x.Contains(
								_azureBlobStorageOptions.PhotosFolderName
							)
					)
					.ToList();

				var additionalDocumentsUrl = request.FileUrl
					.Where(
						x =>
							x.Contains(
								_azureBlobStorageOptions.AdditionalDocumentsFolderName
							)
					)
					.ToList();

				foreach (var photoUrl in photosUrl)
				{
					Uri photoUri = new(photoUrl);
					var fileName = Path.GetFileName(photoUri.LocalPath);

					int lastSlashIndex = photoUrl.LastIndexOf('/');
					var filePath = photoUrl.Substring(
						photoUrl.IndexOf(_azureBlobStorageOptions.ContainerName)
							+ _azureBlobStorageOptions.ContainerName.Length
							+ 1,
						lastSlashIndex
							- (
								photoUrl.IndexOf(_azureBlobStorageOptions.ContainerName)
								+ _azureBlobStorageOptions.ContainerName.Length
								+ 1
							)
					);

					await _blobService.DeleteFileBlobAsync(filePath, fileName);

					var photoFile = await _fileRepository.GetAsync(
						predicate: x => x.LotId == request.LotId && x.FileName == fileName,
						cancellationToken: cancellationToken
					);

					if(photoFile != null)
						await _fileRepository.DeleteAsync(photoFile);
				}

				foreach (var additionalDocumentUrl in additionalDocumentsUrl)
				{
					Uri additionalDocumentUri = new(additionalDocumentUrl);
					var fileName = Path.GetFileName(additionalDocumentUri.LocalPath);
					
					int lastSlashIndex = additionalDocumentUrl.LastIndexOf('/');
					var filePath = additionalDocumentUrl.Substring(
						additionalDocumentUrl.IndexOf(_azureBlobStorageOptions.ContainerName)
							+ _azureBlobStorageOptions.ContainerName.Length
							+ 1,
						lastSlashIndex
							- (
								additionalDocumentUrl.IndexOf(
									_azureBlobStorageOptions.ContainerName
								)
								+ _azureBlobStorageOptions.ContainerName.Length
								+ 1
							)
					);

					await _blobService.DeleteFileBlobAsync(filePath, fileName);

					var additionalDocumentFile = await _fileRepository.GetAsync(
						predicate: x => x.LotId == request.LotId && x.FileName == fileName,
						cancellationToken: cancellationToken
					);

					if(additionalDocumentFile != null)
						await _fileRepository.DeleteAsync(additionalDocumentFile);
				}

				bool wasDeleted = true;

				var response = _mapper.Map<DeletedLotFileResponse>(request);

				response.WasDeleted = wasDeleted;

				return response;
			}
		}
	}
}
