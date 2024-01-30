using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Rates.Queries.GetReceiverRates
{
	public class GetAllReceiverRatesQuery
		: IRequest<GetListResponseDto<GetAllReceiverRatesResponse>>
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetAllReceiverRatesQueryHandler
		: IRequestHandler<GetAllReceiverRatesQuery, GetListResponseDto<GetAllReceiverRatesResponse>>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IBlobService _blobService;

		public GetAllReceiverRatesQueryHandler(
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IMapper mapper,
			IRateRepository rateRepository,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IBlobService blobService
		)
		{
			_currentUserService = currentUserService;
			_userManager = userManager;
			_mapper = mapper;
			_rateRepository = rateRepository;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_blobService = blobService;
		}

		public async Task<GetListResponseDto<GetAllReceiverRatesResponse>> Handle(
			GetAllReceiverRatesQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var feedbacks = await _rateRepository.GetListAsync(
				predicate: r => r.SenderId == user.Id,
				include: x => x.Include(u => u.Receiver),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken
			);

			var response = _mapper.Map<GetListResponseDto<GetAllReceiverRatesResponse>>(feedbacks);

			if (feedbacks.Count > 0)
			{
				foreach (var rate in response.Items)
				{
					if (rate.Receiver != null)
					{
						var receiver = await _userManager.FindByIdAsync(
							rate.Receiver.Id.ToString()
						);

						if (receiver != null && receiver.ProfilePicture != null)
						{
							var profilePictureUrl = _blobService.GetBlobUrl(
								_azureBlobStorageOptions.UserProfilePhotosFolderName,
								receiver.ProfilePicture
							);

							rate.Receiver.ProfilePicture = profilePictureUrl;
						}
					}
				}
			}

			return response;
		}
	}
}
