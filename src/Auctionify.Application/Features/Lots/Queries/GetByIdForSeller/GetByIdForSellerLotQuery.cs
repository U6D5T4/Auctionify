using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Queries.GetByIdForSeller
{
	public class GetByIdForSellerLotQuery : IRequest<GetByIdForSellerLotResponse>
	{
		public int Id { get; set; }

		public class GetByIdForSellerLotQueryHandler
			: IRequestHandler<GetByIdForSellerLotQuery, GetByIdForSellerLotResponse>
		{
			private readonly ILotRepository _lotRepository;
			private readonly IMapper _mapper;
			private readonly IBlobService _blobService;
			private readonly IFileRepository _fileRepository;
			private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

			public GetByIdForSellerLotQueryHandler(
				ILotRepository lotRepository,
				IMapper mapper,
				IBlobService blobService,
				IFileRepository fileRepository,
				IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
			)
			{
				_lotRepository = lotRepository;
				_mapper = mapper;
				_blobService = blobService;
				_fileRepository = fileRepository;
				_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			}

			public async Task<GetByIdForSellerLotResponse> Handle(
				GetByIdForSellerLotQuery request,
				CancellationToken cancellationToken
			)
			{
				var lot = await _lotRepository.GetAsync(
					predicate: x => x.Id == request.Id,
					include: x =>
						x.Include(x => x.Category)
							.Include(x => x.Currency)
							.Include(x => x.Location)
							.Include(x => x.LotStatus)
							.Include(x => x.Bids),
					cancellationToken: cancellationToken
				);

				var result = _mapper.Map<GetByIdForSellerLotResponse>(lot);

				if (lot != null)
				{
					var photos = await _fileRepository.GetListAsync(
						predicate: x =>
							x.LotId == lot.Id
							&& x.Path.Contains(_azureBlobStorageOptions.PhotosFolderName),
						cancellationToken: cancellationToken
					);
					var additionalDocuments = await _fileRepository.GetListAsync(
						predicate: x =>
							x.LotId == lot.Id
							&& x.Path.Contains(
								_azureBlobStorageOptions.AdditionalDocumentsFolderName
							),
						cancellationToken: cancellationToken
					);

					var photoLinks = new List<string>();
					var additionalDocumentLinks = new List<string>();

					foreach (var photo in photos.Items)
					{
						var linkToPhoto = _blobService.GetBlobUrl(photo.Path, photo.FileName);
						photoLinks.Add(linkToPhoto);
					}

					foreach (var additionalDocument in additionalDocuments.Items)
					{
						var linkToAdditionalDocument = _blobService.GetBlobUrl(
							additionalDocument.Path,
							additionalDocument.FileName
						);
						additionalDocumentLinks.Add(linkToAdditionalDocument);
					}

					result.PhotosUrl = photoLinks;
					result.AdditionalDocumentsUrl = additionalDocumentLinks;
				}

				return result;
			}
		}
	}
}
