﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Users.Queries.GetSeller
{
	public class GetSellerQuery : IRequest<GetSellerResponse> { }

	public class GetSellerQueryHandler : IRequestHandler<GetSellerQuery, GetSellerResponse>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;

		public GetSellerQueryHandler(
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IMapper mapper,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository
		)
		{
			_currentUserService = currentUserService;
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_mapper = mapper;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
		}

		public async Task<GetSellerResponse> Handle(
			GetSellerQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

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
				cancellationToken: cancellationToken
			);

			response.CreatedLotsCount = lots.Items.Count;

			var lotStatusIds = await _lotStatusRepository.GetListAsync(
				 predicate: lotStatus =>
					 lotStatus.Name == AuctionStatus.Sold.ToString()
					 || lotStatus.Name == AuctionStatus.NotSold.ToString()
					 || lotStatus.Name == AuctionStatus.Archive.ToString(),
				 cancellationToken: cancellationToken
			 );

			var finishedLots = lots.Items.Where(
				lot => lotStatusIds.Items.Any(lotStatus => lotStatus.Id == lot.LotStatusId)
			);

			response.FinishedLotsCount = finishedLots.Count();

			return response;
		}
	}
}