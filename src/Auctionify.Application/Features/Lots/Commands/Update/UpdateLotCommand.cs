using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
	public class UpdateLotCommand : IRequest<UpdatedLotResponse>, ILotCommandsValidator
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public decimal? StartingPrice { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int? CategoryId { get; set; }
		public string City { get; set; }
		public string? State { get; set; }
		public string Country { get; set; }
		public string Address { get; set; }
		public int? CurrencyId { get; set; }
		public IList<IFormFile>? Photos { get; set; }
		public IList<IFormFile>? AdditionalDocuments { get; set; }
		public bool IsDraft { get; set; }
	}

	public class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand, UpdatedLotResponse>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IMapper _mapper;
		private readonly IBlobService _blobService;
		private readonly IFileRepository _fileRepository;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

		public UpdateLotCommandHandler(
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			IMapper mapper,
			IBlobService blobService,
			IFileRepository fileRepository,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
		)
		{
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_mapper = mapper;
			_blobService = blobService;
			_fileRepository = fileRepository;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
		}

		public async Task<UpdatedLotResponse> Handle(
			UpdateLotCommand request,
			CancellationToken cancellationToken
		)
		{
			AuctionStatus status = request.IsDraft ? AuctionStatus.Draft : AuctionStatus.Upcoming;

			var lotStatus = await _lotStatusRepository.GetAsync(
				s => s.Name == status.ToString(),
				cancellationToken: cancellationToken
			);

			var lot = await _lotRepository.GetAsync(
				l => l.Id == request.Id,
				include: x => x.Include(l => l.Location)!,
				cancellationToken: cancellationToken,
				enableTracking: false
			);

			lot!.LotStatus = lotStatus!;

			var lotUpdated = _mapper.Map(request, lot);

			var createdPhotos = new List<FileDto>();
			var createdAdditionalDocuments = new List<FileDto>();

			if (request.Photos != null)
			{
				var folderPath = _azureBlobStorageOptions.PhotosFolderName;

				var existingPhotos = await _fileRepository.GetListAsync(
					predicate: x => x.LotId == lot.Id && x.Path.Contains(folderPath),
					cancellationToken: cancellationToken
				);

				if (existingPhotos.Count > 0)
				{
					folderPath = existingPhotos.Items[0].Path;
				}
				else
				{
					var folderName = Guid.NewGuid().ToString();
					folderPath = $"{folderPath}/{folderName}";
				}

				foreach (var photo in request.Photos)
				{
					await _blobService.UploadFileBlobAsync(photo, folderPath);

					var file = new Core.Entities.File
					{
						FileName = photo.FileName,
						Path = folderPath,
						LotId = lot.Id,
					};

					var res = await _fileRepository.AddAsync(file);

					createdPhotos.Add(_mapper.Map<FileDto>(res));
				}
				createdPhotos.AddRange(_mapper.Map<IEnumerable<FileDto>>(existingPhotos.Items));
			}

			if (request.AdditionalDocuments != null)
			{
				var folderPath = _azureBlobStorageOptions.AdditionalDocumentsFolderName;

				var existingAdditionalDocuments = await _fileRepository.GetListAsync(
					predicate: x => x.LotId == lot.Id && x.Path.Contains(folderPath),
					cancellationToken: cancellationToken
				);

				if (existingAdditionalDocuments.Count > 0)
				{
					folderPath = existingAdditionalDocuments.Items[0].Path;
				}
				else
				{
					var folderName = Guid.NewGuid().ToString();
					folderPath = $"{folderPath}/{folderName}";
				}

				foreach (var additionalDocument in request.AdditionalDocuments)
				{
					await _blobService.UploadFileBlobAsync(additionalDocument, folderPath);

					var file = new Core.Entities.File
					{
						FileName = additionalDocument.FileName,
						Path = folderPath,
						LotId = lot.Id,
					};

					var res = await _fileRepository.AddAsync(file);

					createdAdditionalDocuments.Add(_mapper.Map<FileDto>(res));
				}

				createdAdditionalDocuments.AddRange(
					_mapper.Map<IEnumerable<FileDto>>(existingAdditionalDocuments.Items)
				);
			}

			await _lotRepository.UpdateAsync(lotUpdated);

			var mappedLot = _mapper.Map<UpdatedLotResponse>(lotUpdated);

			mappedLot.Photos = createdPhotos;
			mappedLot.AdditionalDocuments = createdAdditionalDocuments;

			return mappedLot;
		}
	}
}
