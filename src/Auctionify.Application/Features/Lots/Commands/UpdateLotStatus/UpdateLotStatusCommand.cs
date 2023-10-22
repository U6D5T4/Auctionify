using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
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
			var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, include: x => x.Include(x => x.LotStatus), cancellationToken: cancellationToken);

			var lotStatus = await _lotStatusRepository.GetAsync(predicate: s => s.Name == request.Name, cancellationToken: cancellationToken);
			
			if (lotStatus!.Name == AuctionStatus.Active.ToString() && lot!.LotStatus.Name == AuctionStatus.Upcoming.ToString())
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.Cancelled.ToString() && (lot!.LotStatus.Name == AuctionStatus.Active.ToString() ||
																				lot!.LotStatus.Name == AuctionStatus.Upcoming.ToString()))
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.Sold.ToString() && lot!.LotStatus.Name == AuctionStatus.Active.ToString())
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.NotSold.ToString() && lot!.LotStatus.Name == AuctionStatus.Active.ToString())
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.Reopened.ToString() && lot!.LotStatus.Name == AuctionStatus.Sold.ToString())
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.Draft.ToString() && lot!.LotStatus.Name == AuctionStatus.Draft.ToString())
			{
				lot!.LotStatusId = lotStatus!.Id;
			}
			else if (lotStatus!.Name == AuctionStatus.Upcoming.ToString() && (lot!.LotStatus.Name == AuctionStatus.Draft.ToString() || 
																			  lot!.LotStatus.Name == AuctionStatus.Reopened.ToString() || 
																			  lot!.LotStatus.Name == AuctionStatus.Upcoming.ToString()))
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
