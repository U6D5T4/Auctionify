using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Lots.Commands.Delete
{
	public class DeleteLotCommand : IRequest<DeletedLotResponse>
	{
		public int Id { get; set; }
	}

	public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand, DeletedLotResponse>
	{
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IFileRepository _fileRepository;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IJobSchedulerService _jobSchedulerService;

		public DeleteLotCommandHandler(
			IMapper mapper,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			IBidRepository bidRepository,
			IFileRepository fileRepository,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IJobSchedulerService jobSchedulerService
		)
		{
			_mapper = mapper;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_bidRepository = bidRepository;
			_fileRepository = fileRepository;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_jobSchedulerService = jobSchedulerService;
		}

		public async Task<DeletedLotResponse> Handle(
			DeleteLotCommand request,
			CancellationToken cancellationToken
		)
		{
			var lot = await _lotRepository.GetAsync(
				predicate: x => x.Id == request.Id,
				cancellationToken: cancellationToken,
				include: x => x.Include(x => x.LotStatus).Include(x => x.Bids)
			);

			var currentLotStatus = lot!.LotStatus.Name;

			var newLotStatus = await _lotStatusRepository.GetAsync(
				predicate: s => s.Name == AuctionStatus.Cancelled.ToString(),
				cancellationToken: cancellationToken
			);

			if (currentLotStatus == AuctionStatus.Active.ToString())
			{
				lot!.LotStatusId = newLotStatus!.Id;

				if (lot!.Bids.Any())
					await _bidRepository.DeleteRangeAsync(lot!.Bids.ToList());

				await _lotRepository.UpdateAsync(lot!);

				var updateStatusResponse = _mapper.Map<DeletedLotResponse>(lot);

				updateStatusResponse.WasDeleted = false;

				await _jobSchedulerService.RemoveLotFinishJob(lot.Id);

				return updateStatusResponse;
			}

			var photos = await _fileRepository.GetListAsync(
				predicate: x =>
					x.LotId == lot.Id
					&& x.Path.Contains(_azureBlobStorageOptions.PhotosFolderName),
				cancellationToken: cancellationToken
			);

			var additionalDocuments = await _fileRepository.GetListAsync(
				predicate: x =>
					x.LotId == lot.Id
					&& x.Path.Contains(_azureBlobStorageOptions.AdditionalDocumentsFolderName),
				cancellationToken: cancellationToken
			);

			if (photos.Count > 0)
			{
				foreach (var photo in photos.Items)
				{
					await _blobService.DeleteFileBlobAsync(photo.Path, photo.FileName);
				}
			}

			if (additionalDocuments.Count > 0)
			{
				foreach (var additionalDocument in additionalDocuments.Items)
				{
					await _blobService.DeleteFileBlobAsync(
						additionalDocument.Path,
						additionalDocument.FileName
					);
				}
			}

			await _jobSchedulerService.RemoveUpcomingToActiveJob(lot.Id);

			await _lotRepository.DeleteAsync(lot);

			bool wasDeleted = true;

			var response = _mapper.Map<DeletedLotResponse>(lot);

			response.WasDeleted = wasDeleted;

			return response;
		}
	}
}
