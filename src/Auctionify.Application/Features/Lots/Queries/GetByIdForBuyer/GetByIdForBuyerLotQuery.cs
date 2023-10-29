using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Queries.GetByIdForBuyer
{
	public class GetByIdForBuyerLotQuery : IRequest<GetByIdForBuyerLotResponse>
	{
		public int Id { get; set; }

		public class GetByIdForBuyerLotQueryHandler
			: IRequestHandler<GetByIdForBuyerLotQuery, GetByIdForBuyerLotResponse>
		{
			private readonly ILotRepository _lotRepository;
			private readonly IMapper _mapper;
			private readonly IBlobService _blobService;
			private readonly IFileRepository _fileRepository;
			private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
			private readonly IWatchlistRepository _watchlistRepository;
			private readonly ICurrentUserService _currentUserService;
			private readonly UserManager<User> _userManager;

			public GetByIdForBuyerLotQueryHandler(
				ILotRepository lotRepository,
				IMapper mapper,
				IBlobService blobService,
				IFileRepository fileRepository,
				IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
				IWatchlistRepository watchlistRepository,
				ICurrentUserService currentUserService,
				UserManager<User> userManager
			)
			{
				_lotRepository = lotRepository;
				_mapper = mapper;
				_blobService = blobService;
				_fileRepository = fileRepository;
				_azureBlobStorageOptions = azureBlobStorageOptions.Value;
				_watchlistRepository = watchlistRepository;
				_currentUserService = currentUserService;
				_userManager = userManager;
			}

			public async Task<GetByIdForBuyerLotResponse> Handle(
				GetByIdForBuyerLotQuery request,
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

				var result = _mapper.Map<GetByIdForBuyerLotResponse>(lot);

				if (lot != null)
				{
					var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);
					var isInWatchlist = await _watchlistRepository.GetAsync(
						predicate: x => x.UserId == user!.Id && x.LotId == lot.Id,
						cancellationToken: cancellationToken
					);

					result.IsInWatchlist = isInWatchlist != null;

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
