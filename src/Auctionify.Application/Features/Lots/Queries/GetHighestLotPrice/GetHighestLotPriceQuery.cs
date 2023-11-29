using Auctionify.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetHighestLotPrice
{
	public class GetHighestLotPriceQuery : IRequest<decimal>
	{
	}

	public class GetHighestLotPriceQueryHandler : IRequestHandler<GetHighestLotPriceQuery, decimal>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;

		public GetHighestLotPriceQueryHandler(ILotRepository lotRepository, IBidRepository bidRepository)
		{
			_lotRepository = lotRepository;
			_bidRepository = bidRepository;
		}

		public async Task<decimal> Handle(GetHighestLotPriceQuery request, CancellationToken cancellationToken)
		{
			var maxBidPriceValue = _bidRepository.Query().Any()
				? await _bidRepository.Query().MaxAsync(b => b.NewPrice, cancellationToken)
				: decimal.MinValue;

			var maxLotPriceValue = _lotRepository.Query().Any()
				? (decimal) await _lotRepository.Query().MaxAsync(l => l.StartingPrice, cancellationToken)
				: decimal.MinValue;

			var maxValue = Math.Max(maxBidPriceValue, maxLotPriceValue);

			return maxValue == decimal.MinValue ? 1000000 : maxValue;
		}
	}
}
