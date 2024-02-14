using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using User = Auctionify.Core.Entities.User;

namespace Auctionify.Application.Features.Rates.Queries.GetSenderRate
{
	public class GetSenderRateQuery : IRequest<GetSenderRateResponse>
	{
		public int LotId { get; set; }
	}

	public class GetSenderRateQueryHandler
		: IRequestHandler<GetSenderRateQuery, GetSenderRateResponse>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IBlobService _blobService;

		public GetSenderRateQueryHandler(
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

		public async Task<GetSenderRateResponse> Handle(
			GetSenderRateQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var senderRate = await _rateRepository.GetAsync(
				predicate: r => r.SenderId == user.Id && r.LotId == request.LotId,
				enableTracking: false
			);

			return _mapper.Map<GetSenderRateResponse>(senderRate);

		}
	}
}
