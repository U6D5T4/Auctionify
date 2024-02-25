using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Reports.Query.GetUserIncome
{
	public class GetUserIncomeQuery : IRequest<IList<GetUserIncomeResponse>>
	{
		public AnalyticReportPeriod Period { get; set; }

		public int PeriodNumber { get; set; }
	}

	public class GetUserIncomeQueryHandler : IRequestHandler<GetUserIncomeQuery, IList<GetUserIncomeResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly IBidRepository _bidRepository;

		public GetUserIncomeQueryHandler(ILotRepository lotRepository, ICurrentUserService currentUserService, IBidRepository bidRepository)
		{
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
			_bidRepository = bidRepository;
		}

		public async Task<IList<GetUserIncomeResponse>> Handle(GetUserIncomeQuery request, CancellationToken cancellationToken)
		{
			var query = _lotRepository.Query().Include(l => l.Seller).Include(l => l.Bids).Include(l => l.LotStatus)
				.Where(l => l.Seller.Email == _currentUserService.UserEmail && l.LotStatus.Name == AuctionStatus.Sold.ToString());

			query = request.Period switch
			{
				AnalyticReportPeriod.Day => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddDays(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Week => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddDays(-(request.PeriodNumber * 7)) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Month => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddMonths(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Year => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddYears(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				_ => query,
			};

			var lots = query.ToList();

			var highestBids = lots.Select(item =>
			{
				var highestBid = _bidRepository.Query()
											   .Where(b => b.LotId == item.Id)
											   .OrderBy(b => b.NewPrice)
											   .LastOrDefault();
				return new
				{
					LotId = item.Id,
					HighestBid = highestBid?.NewPrice ?? 0
				};
			});

			var result = (from data in lots
						  group data by data.EndDate.Date into grouped
						  select new GetUserIncomeResponse
						  {
							  Date = grouped.Key,
							  Amount = highestBids.Where(x => x.LotId == grouped.Last().Id).Select(x => x.HighestBid).FirstOrDefault()
						  }).ToList();

			return result;
		}
	}
}
