using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.Delete
{
	public class DeleteLotCommand : IRequest<DeletedLotResponse>
	{
		public int Id { get; set; }

		public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand, DeletedLotResponse>
		{
			private readonly ILotRepository _lotRepository;
			private readonly IMapper _mapper;
			private readonly ILotStatusRepository _lotStatusRepository;

			public DeleteLotCommandHandler(ILotRepository lotRepository, IMapper mapper, ILotStatusRepository lotStatusRepository)
			{
				_lotRepository = lotRepository;
				_mapper = mapper;
				_lotStatusRepository = lotStatusRepository;
			}

			public async Task<DeletedLotResponse> Handle(DeleteLotCommand request, CancellationToken cancellationToken)
			{
				var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken, include: x => x.Include(x => x.LotStatus));

				var currentLotStatus = lot!.LotStatus.Name;

				var newLotStatus = await _lotStatusRepository.GetAsync(predicate: s => s.Name == AuctionStatus.Cancelled.ToString(), cancellationToken: cancellationToken);

				if (currentLotStatus == AuctionStatus.Active.ToString())
				{
					lot!.LotStatusId = newLotStatus!.Id;

					await _lotRepository.UpdateAsync(lot!);

					var updateStatusResponse = _mapper.Map<DeletedLotResponse>(lot);

					updateStatusResponse.WasDeleted = false;

					return updateStatusResponse;
				}

				await _lotRepository.DeleteAsync(lot!);

				bool wasDeleted = true;

				var response = _mapper.Map<DeletedLotResponse>(lot);

				response.WasDeleted = wasDeleted;

				return response;
			}
		}

	}
}
