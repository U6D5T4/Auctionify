using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Queries.GetAll
{
	public class GetAllLotsQuery : IRequest<List<GetAllLotsResponse>> { }

	public class GetAllLotsQueryHandler : IRequestHandler<GetAllLotsQuery, List<GetAllLotsResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IFileRepository _fileRepository;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

		public GetAllLotsQueryHandler(
			ILotRepository lotRepository,
			IMapper mapper,
			IFileRepository fileRepository,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
		)
		{
			_lotRepository = lotRepository;
			_mapper = mapper;
			_fileRepository = fileRepository;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
		}

		public async Task<List<GetAllLotsResponse>> Handle(
			GetAllLotsQuery request,
			CancellationToken cancellationToken
		)
		{
			var lots = await _lotRepository.GetListAsync(cancellationToken: cancellationToken);

			var response = new List<GetAllLotsResponse>();

			foreach (var lot in lots)
			{
				var photo = await _fileRepository.GetAsync(
					predicate: x =>
						x.LotId == lot.Id
						&& x.Path.Contains(_azureBlobStorageOptions.PhotosFolderName),
					cancellationToken: cancellationToken
				);

				var lotResponse = _mapper.Map<GetAllLotsResponse>(lot);

				if (photo != null)
				{
					var linkToPhoto = _blobService.GetBlobUrl(photo.Path, photo.FileName);

					lotResponse.MainPhotoUrl = linkToPhoto;
				}

				response.Add(lotResponse);
			}
			return response;
		}
	}
}
