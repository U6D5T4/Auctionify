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

namespace Auctionify.Application.Features.Users.Queries.GetBuyer
{
	public class GetBuyerQuery : IRequest<GetBuyerResponse> 
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetBuyerQueryHandler
		: IRequestHandler<GetBuyerQuery, GetBuyerResponse>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;

		public GetBuyerQueryHandler(
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IMapper mapper,
			IRateRepository rateRepository

		)
		{
			_currentUserService = currentUserService;
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_mapper = mapper;
			_rateRepository = rateRepository;
		}

		public async Task<GetBuyerResponse> Handle(
			GetBuyerQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var profilePictureName = user.ProfilePicture;

			var response = _mapper.Map<GetBuyerResponse>(user);

			if (profilePictureName != null)
			{
				var profilePictureUrl = _blobService.GetBlobUrl(
					_azureBlobStorageOptions.UserProfilePhotosFolderName,
					profilePictureName
				);

				response.ProfilePictureUrl = profilePictureUrl;
			}

			var ratesForUser = await _rateRepository.GetListAsync(predicate: r => r.ReceiverId == user.Id,
				include: x =>
					x.Include(u => u.Sender),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken);

			var feedbacks = await _rateRepository.GetListAsync(predicate: r => r.SenderId == user.Id,
				include: x =>
					x.Include(u => u.Receiver),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken);

			response.AverageRate = ratesForUser.Items.Average(rate => rate.RatingValue);
			response.RatesCount = ratesForUser.Items.Count();

			return response;
		}
	}
}
