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

namespace Auctionify.Application.Features.Rates.Queries.GetSenderRates
{
	public class GetAllSenderRatesQuery : IRequest<GetListResponseDto<GetAllSenderRatesResponse>>
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetAllSenderRatesQueryHandler
		: IRequestHandler<GetAllSenderRatesQuery, GetListResponseDto<GetAllSenderRatesResponse>>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IBlobService _blobService;

		public GetAllSenderRatesQueryHandler(
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

		public async Task<GetListResponseDto<GetAllSenderRatesResponse>> Handle(
			GetAllSenderRatesQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var userRate = await _rateRepository.GetListAsync(
				predicate: r => r.ReceiverId == user.Id,
				include: x => x.Include(u => u.Sender),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken
			);

			var response = _mapper.Map<GetListResponseDto<GetAllSenderRatesResponse>>(userRate);

			if (userRate.Count > 0)
			{
				foreach (var rate in response.Items)
				{
					if (rate.Sender != null)
					{
						var sender = await _userManager.FindByIdAsync(rate.Sender.Id.ToString());

						if (sender != null && sender.ProfilePicture != null)
						{
							var profilePictureUrl = _blobService.GetBlobUrl(
								_azureBlobStorageOptions.UserProfilePhotosFolderName,
								sender.ProfilePicture
							);

							rate.Sender.ProfilePicture = profilePictureUrl;
						}
					}
				}
			}

			return response;
		}
	}
}
