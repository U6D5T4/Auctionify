using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Users.Queries.GetBuyer;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Queries.GetById
{
    public class GetByIdUserQuery : IRequest<GetByIdUserResponse>
    {
        public string Id { get; set; }
    }

    public class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, GetByIdUserResponse>
    {
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;

		public GetByIdUserQueryHandler(
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IMapper mapper,
			IRateRepository rateRepository
		)
        {
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_mapper = mapper;
			_rateRepository = rateRepository;
		}

        public async Task<GetByIdUserResponse> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

			var profilePictureName = user.ProfilePicture;

			var response = _mapper.Map<GetByIdUserResponse>(user);

			if (profilePictureName != null)
			{
				var profilePictureUrl = _blobService.GetBlobUrl(
					_azureBlobStorageOptions.UserProfilePhotosFolderName,
					profilePictureName
				);

				response.ProfilePictureUrl = profilePictureUrl;
			}

			var avg = await _rateRepository.GetListAsync(predicate: r => r.ReceiverId == user.Id,
				include: x =>
					x.Include(u => u.Sender),
				enableTracking: false,
				size: int.MaxValue,
				index: 0,
				cancellationToken: cancellationToken);

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
