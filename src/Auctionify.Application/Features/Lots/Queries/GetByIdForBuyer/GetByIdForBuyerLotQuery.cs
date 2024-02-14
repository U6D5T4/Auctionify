using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Queries.GetByIdForBuyer
{
	public class GetByIdForBuyerLotQuery : IRequest<GetByIdForBuyerLotResponse>
	{
		public int Id { get; set; }
	}

	public class GetByIdForBuyerLotQueryHandler
		: IRequestHandler<GetByIdForBuyerLotQuery, GetByIdForBuyerLotResponse>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IBlobService _blobService;
		private readonly IFileRepository _fileRepository;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IWatchlistService _watchlistService;

		public GetByIdForBuyerLotQueryHandler(
			ILotRepository lotRepository,
			IMapper mapper,
			IBlobService blobService,
			IFileRepository fileRepository,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IWatchlistService watchlistService
		)
		{
			_lotRepository = lotRepository;
			_mapper = mapper;
			_blobService = blobService;
			_fileRepository = fileRepository;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_watchlistService = watchlistService;
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
						.Include(x => x.Bids)
						.Include(x => x.Seller),
				cancellationToken: cancellationToken
			);

			var result = _mapper.Map<GetByIdForBuyerLotResponse>(lot);

			if (lot is not null)
			{
				var user = await _userManager.Users.FirstOrDefaultAsync(
					u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
					cancellationToken: cancellationToken
				);

				result.IsInWatchlist = await _watchlistService.IsLotInUserWatchlist(
					lot.Id,
					user!.Id,
					cancellationToken
				);

				if (lot.Bids.Count > 0)
				{
					var notRemovedBids = lot.Bids.Where(x => !x.BidRemoved)
						.OrderByDescending(x => x.TimeStamp)
						.ToList();

					result.BidCount = notRemovedBids.Count;

					result.Bids = _mapper.Map<List<BidDto>>(notRemovedBids);
				}

				var photos = await _fileRepository.GetListAsync(
					predicate: x =>
						x.LotId == lot.Id
						&& x.Path.Contains(_azureBlobStorageOptions.PhotosFolderName),
					cancellationToken: cancellationToken
				);
				var additionalDocuments = await _fileRepository.GetListAsync(
					predicate: x =>
						x.LotId == lot.Id
						&& x.Path.Contains(_azureBlobStorageOptions.AdditionalDocumentsFolderName),
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

				result.SellerEmail = lot.Seller.Email;

				var profilePictureName = user.ProfilePicture;

				if (user.ProfilePicture != null)
				{
					var profilePictureUrl = _blobService.GetBlobUrl(
						_azureBlobStorageOptions.UserProfilePhotosFolderName,
						profilePictureName
					);

					result.ProfilePictureUrl = profilePictureUrl;
				}
			}
			else
			{
				throw new ValidationException("Lot not found");
			}

			return result;
		}
	}
}
