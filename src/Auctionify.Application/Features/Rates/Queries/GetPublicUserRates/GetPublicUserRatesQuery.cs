using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using User = Auctionify.Core.Entities.User;

namespace Auctionify.Application.Features.Rates.Queries.GetPublicUserRates
{
	public class GetPublicUserRatesQuery : IRequest<GetListResponseDto<GetPublicUserRatesResponse>>
	{
		public string UserId { get; set; }
	}

	public class GetPublicUserRatesQueryHandler
		: IRequestHandler<GetPublicUserRatesQuery, GetListResponseDto<GetPublicUserRatesResponse>>
	{
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;

		public GetPublicUserRatesQueryHandler(
			UserManager<User> userManager,
			IMapper mapper,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IRateRepository rateRepository
		)
		{
			_userManager = userManager;
			_mapper = mapper;
			_blobService = blobService;

			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_rateRepository = rateRepository;
		}

		public async Task<GetListResponseDto<GetPublicUserRatesResponse>> Handle(
			GetPublicUserRatesQuery request,
			CancellationToken cancellationToken
		)
		{
			var publicUser = await _userManager.FindByIdAsync(request.UserId);

			if (publicUser is not null)
			{
				var publicUserRates = await _rateRepository.GetUnpaginatedListAsync(
					predicate: r => r.ReceiverId == publicUser.Id,
					include: x => x.Include(u => u.Sender),
					enableTracking: false,
					orderBy: x => x.OrderByDescending(r => r.CreationDate),
					cancellationToken: cancellationToken
				);

				var response = _mapper.Map<List<GetPublicUserRatesResponse>>(publicUserRates);

				foreach (var item in response)
				{
					if (item.Sender.ProfilePicture != null)
					{
						var profilePictureUrl = _blobService.GetBlobUrl(
							_azureBlobStorageOptions.UserProfilePhotosFolderName,
							item.Sender.ProfilePicture
						);

						item.Sender.ProfilePicture = profilePictureUrl;
					}
				}

				return new GetListResponseDto<GetPublicUserRatesResponse>
				{
					Items = response
				};
			}

			return new GetListResponseDto<GetPublicUserRatesResponse>
			{
				Items = new List<GetPublicUserRatesResponse>()
			};
		}
	}
}
