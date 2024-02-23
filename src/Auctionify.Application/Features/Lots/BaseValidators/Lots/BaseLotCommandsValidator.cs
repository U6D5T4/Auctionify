using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using FluentValidation;

namespace Auctionify.Application.Features.Lots.BaseValidators.Lots
{
	public class BaseLotCommandsValidator : AbstractValidator<ILotCommandsValidator>
	{
		private readonly int minimumStartDateAndDateNowDiff = 24;
		private readonly int minimumEndDateAndStartDateDiff = 4;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly ICurrencyRepository _currencyRepository;

		public BaseLotCommandsValidator(
			ICategoryRepository categoryRepository,
			ILotStatusRepository lotStatusRepository,
			ICurrencyRepository currencyRepository
		)
		{
			_categoryRepository = categoryRepository;
			_lotStatusRepository = lotStatusRepository;
			_currencyRepository = currencyRepository;

			RuleFor(l => l.IsDraft)
				.MustAsync(
					async (l, cancellationToken) =>
					{
						AuctionStatus status = l ? AuctionStatus.Draft : AuctionStatus.Upcoming;

						var lotStatus = await _lotStatusRepository.GetAsync(
							s => s.Name == status.ToString(),
							cancellationToken: cancellationToken
						);

						if (lotStatus == null)
							return false;

						return true;
					}
				)
				.WithMessage("Lot status was not created in DB");

			RuleFor(l => l.Title)
				.MinimumLength(6)
				.MaximumLength(64)
				.Unless(l => string.IsNullOrEmpty(l.Title))
				.WithMessage("Title has to be minimum 6 symbols and maximum 64");

			RuleFor(l => l.Description)
				.MinimumLength(30)
				.MaximumLength(500)
				.Unless(l => string.IsNullOrEmpty(l.Description))
				.WithMessage("Description has to be minimum 30 symbols and maximum 500");

			RuleFor(l => l.StartingPrice)
				.NotEmpty()
				.GreaterThan(0)
				.WithMessage("Starting price has to be greater than 0")
				.Unless(l => l.StartingPrice == null && l.IsDraft != false);

			RuleFor(l => l.CategoryId)
				.MustAsync(
					async (categoryId, cancellationToken) =>
					{
						var category = await _categoryRepository.GetAsync(
							c => c.Id == categoryId,
							cancellationToken: cancellationToken
						);
						if (category == null)
							return false;

						return true;
					}
				)
				.Unless(l => l.CategoryId == null)
				.WithMessage("The category entered does not exist in the system");

			RuleFor(l => l.CurrencyId)
				.MustAsync(
					async (currencyId, cancellationToken) =>
					{
						var currency = await _currencyRepository.GetAsync(
							c => c.Id == currencyId,
							cancellationToken: cancellationToken
						);
						if (currency == null)
							return false;

						return true;
					}
				)
				.Unless(l => l.CurrencyId == null)
				.WithMessage("Currency entered does not exist in the system");

			RuleFor(l => l.City).NotEmpty();

			RuleFor(l => l.Country).NotEmpty();

			RuleFor(l => l.Address).NotEmpty();

			    RuleFor(l => l.Longitude)
					    .NotEmpty();

			    RuleFor(l => l.Latitude)
					    .NotEmpty();

			ConfigureValidationWhenConcreteCreating();
        }

		private void ConfigureValidationWhenConcreteCreating()
		{
			RuleFor(l => l.StartDate)
				.NotEmpty()
				.Must(time =>
				{
					if (time > DateTime.UtcNow)
					{
						var timeDifference = time - DateTime.UtcNow;

						if (timeDifference.TotalHours >= minimumStartDateAndDateNowDiff)
							return true;
					}

					return false;
				})
				.When(l => !l.IsDraft)
				.WithMessage("Start date has to be at least 24 hours ahead than now");

			RuleFor(l => l.EndDate)
				.Must(
					(lot, time) =>
					{
						var timeDifference = time - lot.StartDate;
						if (timeDifference.TotalHours >= minimumEndDateAndStartDateDiff)
							return true;

						return false;
					}
				)
				.When(l => !l.IsDraft)
				.When(l => l.EndDate >= DateTime.MinValue)
				.WithMessage("End date has to be at least 4 hours ahead than start date");
		}
	}
}
