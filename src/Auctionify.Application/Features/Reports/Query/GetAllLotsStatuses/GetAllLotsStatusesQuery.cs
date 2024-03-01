using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Reports.Query.GetAllLotsStatuses
{
	public class GetAllLotsStatusesQuery : IRequest<IList<GetAllLotsStatusesQueryResponse>> { }

	public class GetAllLotsStatusesQueryHandler
		: IRequestHandler<GetAllLotsStatusesQuery, IList<GetAllLotsStatusesQueryResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;

		public GetAllLotsStatusesQueryHandler(
			ILotRepository lotRepository,
			ICurrentUserService currentUserService
		)
		{
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
		}

		public async Task<IList<GetAllLotsStatusesQueryResponse>> Handle(
			GetAllLotsStatusesQuery request,
			CancellationToken cancellationToken
		)
		{
			var query = _lotRepository
				.QueryAsNoTracking()
				.Where(l => l.Seller.Email == _currentUserService.UserEmail);

			var result = (
				from data in query
				group data by data.LotStatus into grouped
				select new GetAllLotsStatusesQueryResponse
				{
					Status = grouped.Key.Name,
					Count = grouped.Count()
				}
			).ToListAsync(cancellationToken: cancellationToken);

			return await result;
		}
	}
}
