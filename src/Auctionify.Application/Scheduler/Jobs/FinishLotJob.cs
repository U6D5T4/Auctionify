using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Auctionify.Application.Scheduler.Jobs
{
	public class FinishLotJob : IJob
	{
		private readonly ILotRepository _lotRepository;
		private readonly IMediator _mediator;

		public FinishLotJob(ILotRepository lotRepository,
			IMediator mediator)
		{
			_lotRepository = lotRepository;
			_mediator = mediator;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var lotId = context.MergedJobDataMap.GetInt("lotId");

			var lot = await _lotRepository.GetAsync(x => x.Id == lotId,
				include: x =>
							x.Include(l => l.LotStatus)
							.Include(l => l.Bids));
			
			if (lot == null) { return; }

			Enum.TryParse(lot.LotStatus.Name, out AuctionStatus lotStatus);

			AuctionStatus futureStatus = AuctionStatus.NotSold;

			if (lot.Bids.Count > 0)
			{
				futureStatus = AuctionStatus.Sold;
			}

			var result = await _mediator.Send(new UpdateLotStatusCommand { Id = lotId, Name = futureStatus.ToString() });
		}
	}
}
