using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.UpdateLotStatus
{
	public class UpdateLotStatusCommand: IRequest<UpdateLotStatusResponse>
	{
		public int Id { get; set; }

		public string? Name { get; set; }
	}

	public class UpdateLotStatusCommandHandler : IRequestHandler<UpdateLotStatusCommand, UpdateLotStatusResponse>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IMapper _mapper;

		public UpdateLotStatusCommandHandler(ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository, IMapper mapper)
		{
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_mapper = mapper;
		}

		public async Task<UpdateLotStatusResponse> Handle(UpdateLotStatusCommand request, CancellationToken cancellationToken)
		{
			var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, 
													  include: x => x.Include(x => x.LotStatus), 
											cancellationToken: cancellationToken);

			var lotStatus = await _lotStatusRepository.GetAsync(predicate: s => s.Name == request.Name, 
														cancellationToken: cancellationToken);

			_ = Enum.TryParse(lot!.LotStatus.Name, out AuctionStatus currentLostStatus);
			_ = Enum.TryParse(lotStatus!.Name, out AuctionStatus newLotStatus);

			if (newLotStatus == AuctionStatus.Active && currentLostStatus == AuctionStatus.Upcoming)
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.Cancelled && (currentLostStatus == AuctionStatus.Active ||
																 currentLostStatus == AuctionStatus.Upcoming))
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.Sold && currentLostStatus == AuctionStatus.Active)
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.NotSold && currentLostStatus == AuctionStatus.Active)
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.Reopened && currentLostStatus == AuctionStatus.Sold)
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.Draft && currentLostStatus == AuctionStatus.Draft)
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (newLotStatus == AuctionStatus.Upcoming && (currentLostStatus == AuctionStatus.Draft ||
																currentLostStatus == AuctionStatus.Reopened ||
																currentLostStatus == AuctionStatus.Upcoming))
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else
			{
				throw new ValidationException("Invalid status change");
			}

			await _lotRepository.UpdateAsync(lot);
			
			var result = _mapper.Map<UpdateLotStatusResponse>(lot);

			return result;
		}
	}
}
