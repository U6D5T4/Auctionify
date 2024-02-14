using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using FluentValidation;

namespace Auctionify.Application.Features.Rates.Commands.AddRateToBuyer
{
	public class AddRateToBuyerCommandValidator : AbstractValidator<AddRateToBuyerCommand>
	{
		private readonly IRateRepository _rateRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;

		public AddRateToBuyerCommandValidator(
			IRateRepository rateRepository,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository
		)
		{
			_rateRepository = rateRepository;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;

			ClassLevelCascadeMode = CascadeMode.Stop;

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
							cancellationToken: cancellationToken
						);

						return lot != null;
					}
				)
				.WithMessage("Lot with this Id does not exist")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
							cancellationToken: cancellationToken
						);

						var lotStatus = await _lotStatusRepository.GetAsync(
							predicate: x => x.Name == AuctionStatus.Sold.ToString(),
							cancellationToken: cancellationToken
						);

						if (lot is not null && lotStatus is not null)
						{
							return lot.LotStatusId == lotStatus.Id;
						}

						return false;
					}
				)
				.WithMessage("You can rate if the lot will be sold to you")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (request, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == request.LotId,
							cancellationToken: cancellationToken
						);

						var ratings = await _rateRepository.GetListAsync(
								predicate: l => l.LotId == request.LotId,
								cancellationToken: cancellationToken
						);

						if (ratings.Items.Count <= 2 && !ratings.Items.Any(item => item.SenderId == lot.SellerId))
						{
							return true;
						}

						return false;
					}
				)
				.WithMessage("You have already rated")
				.OverridePropertyName("Rate")
				.WithName("Rate");
		}
	}
}
