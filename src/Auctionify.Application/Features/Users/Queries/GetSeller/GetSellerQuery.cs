using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Users.Queries.GetSeller
{
	public class GetSellerQuery : IRequest<GetSellerResponse>
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetSellerQueryHandler : IRequestHandler<GetSellerQuery, GetSellerResponse>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IRateRepository _rateRepository;

		public GetSellerQueryHandler(
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IMapper mapper,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			IRateRepository rateRepository
		)
		{
			_currentUserService = currentUserService;
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_mapper = mapper;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_rateRepository = rateRepository;
		}

		public async Task<GetSellerResponse> Handle(
			GetSellerQuery request,
			CancellationToken cancellationToken
		)
		{
			var users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
			var user = users.Find(u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted);

			var profilePictureName = user.ProfilePicture;

			var response = _mapper.Map<GetSellerResponse>(user);

			if (profilePictureName != null)
			{
				var profilePictureUrl = _blobService.GetBlobUrl(
					_azureBlobStorageOptions.UserProfilePhotosFolderName,
					profilePictureName
				);

				response.ProfilePictureUrl = profilePictureUrl;
			}

			var lots = await _lotRepository.GetListAsync(
				predicate: lot => lot.SellerId == user.Id,
				size: int.MaxValue,
				index: 0,
				cancellationToken: cancellationToken
			);

			response.CreatedLotsCount = lots.Items.Count;

			var lotStatusIds = await _lotStatusRepository.GetListAsync(
				predicate: lotStatus =>
					lotStatus.Name == AuctionStatus.Sold.ToString()
					|| lotStatus.Name == AuctionStatus.NotSold.ToString()
					|| lotStatus.Name == AuctionStatus.Archive.ToString(),
				size: int.MaxValue,
				index: 0,
				cancellationToken: cancellationToken
			);

			var finishedLots = lots.Items.Where(
				lot => lotStatusIds.Items.Any(lotStatus => lotStatus.Id == lot.LotStatusId)
			);

			response.FinishedLotsCount = finishedLots.Count();

			var avg = await _rateRepository.GetListAsync(
				predicate: r => r.ReceiverId == user.Id,
				include: x => x.Include(u => u.Sender),
				enableTracking: false,
				size: int.MaxValue,
				index: 0,
				cancellationToken: cancellationToken
			);

			if (avg.Items.Count > 0)
			{
				response.AverageRate = avg.Items.Average(rate => rate.RatingValue);
			}

			response.RatesCount = avg.Items.Count;

			if (avg.Count > 0)
			{
				var starCounts = new Dictionary<byte, int>
				{
					{ 5, 0 },
					{ 4, 0 },
					{ 3, 0 },
					{ 2, 0 },
					{ 1, 0 },
				};

				foreach (var rate in avg.Items)
				{
					if (rate.Sender != null)
					{
						byte ratingValueKey = rate.RatingValue;

						if (starCounts.ContainsKey(ratingValueKey))
						{
							starCounts[ratingValueKey]++;
						}
					}
				}
				response.StarCounts = starCounts;
			}

			return response;
		}
	}
}
