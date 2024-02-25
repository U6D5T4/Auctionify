using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Reports.Query.GetCreatedLotsCount
{
	public class GetCreatedLotsCountQuery : IRequest<GetCreatedLotsCountResponse>
	{
		public AnalyticReportPeriod Period { get; set; }

		public int PeriodNumber { get; set; }
	}

	public class GetCreatedLotsCountQueryHandler : IRequestHandler<GetCreatedLotsCountQuery, GetCreatedLotsCountResponse>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;

		public GetCreatedLotsCountQueryHandler(ILotRepository lotRepository,
			ICurrentUserService currentUserService)
		{
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
		}

		public async Task<GetCreatedLotsCountResponse> Handle(GetCreatedLotsCountQuery request, CancellationToken cancellationToken)
		{
			var query = _lotRepository.Query().Include(l => l.Seller).Where(l => l.Seller.Email == _currentUserService.UserEmail);

			query = request.Period switch
			{
				AnalyticReportPeriod.Day => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddDays(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Week => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddDays(-(request.PeriodNumber * 7)) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Month => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddMonths(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				AnalyticReportPeriod.Year => query.Where(l => l.CreationDate >= DateTime.UtcNow.AddYears(-request.PeriodNumber) && l.CreationDate <= DateTime.UtcNow),
				_ => query,
			};

			var dataResult = (from data in query
						 group data by data.CreationDate.Date into grouped
						 select new CreatedLotsDay
						 {
							 Date = grouped.Key.Date,
							 Count = grouped.Count(),
						 }).ToList();

			var result = new GetCreatedLotsCountResponse
			{
				Period = request.Period,
				Data = dataResult,
			};

			return result;
		}
	}
}
