using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.UpdateLotStatus
{
	public class UpdateLotStatusCommand: IRequest<UpdatedLotStatusResponse>
	{
		public int Id { get; set; }

		public string? Name { get; set; }
	}

	public class UpdateLotStatusCommandHandler : IRequestHandler<UpdateLotStatusCommand, UpdatedLotStatusResponse>
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

		public async Task<UpdatedLotStatusResponse> Handle(UpdateLotStatusCommand request, CancellationToken cancellationToken)
		{
			var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, 
													  include: x => x.Include(x => x.LotStatus), 
											cancellationToken: cancellationToken);

			var lotStatus = await _lotStatusRepository.GetAsync(predicate: s => s.Name == request.Name, 
														cancellationToken: cancellationToken);

			_ = Enum.TryParse(lot!.LotStatus.Name, out AuctionStatus currentLotStatus);
			_ = Enum.TryParse(lotStatus!.Name, out AuctionStatus newLotStatus);

			if (newLotStatus == AuctionStatus.Active && currentLotStatus == AuctionStatus.Upcoming)
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.Cancelled && (currentLotStatus == AuctionStatus.Active ||
																 currentLotStatus == AuctionStatus.Upcoming))
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.Sold && currentLotStatus == AuctionStatus.Active)
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.NotSold && currentLotStatus == AuctionStatus.Active)
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.Reopened && currentLotStatus == AuctionStatus.Sold)
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.Draft && currentLotStatus == AuctionStatus.Draft)
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else if (newLotStatus == AuctionStatus.Upcoming && (currentLotStatus == AuctionStatus.Draft ||
																currentLotStatus == AuctionStatus.Reopened ||
																currentLotStatus == AuctionStatus.Upcoming))
			{
				lot.LotStatusId = lotStatus.Id;
			}
			else
			{
				throw new ValidationException("Invalid status change");
			}

			await _lotRepository.UpdateAsync(lot);
			
			var result = _mapper.Map<UpdatedLotStatusResponse>(lot);

			return result;
		}
	}
}
