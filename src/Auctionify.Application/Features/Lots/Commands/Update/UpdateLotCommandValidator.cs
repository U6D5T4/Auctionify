using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotCommandValidator : AbstractValidator<UpdateLotCommand>
    {
        private readonly int minimumStartDateAndDateNowDiff = 24;
        private readonly int minimumEndDateAndStartDateDiff = 4;
        private readonly ILotRepository _lotRepository;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public UpdateLotCommandValidator(ILotRepository lotRepository,
            ILotStatusRepository lotStatusRepository,
            ICategoryRepository categoryRepository,
            ICurrencyRepository currencyRepository)
        {
            _lotRepository = lotRepository;
            _lotStatusRepository = lotStatusRepository;
            _categoryRepository = categoryRepository;
            _currencyRepository = currencyRepository;

            RuleFor(l => l.IsDraft)
                .MustAsync(async (l, cancellationToken) =>
                {
                    AuctionStatus status = l ? AuctionStatus.Draft : AuctionStatus.Upcoming;

                    var lotStatus = await _lotStatusRepository.GetAsync(s => s.Name == status.ToString(), cancellationToken: cancellationToken);

                    if (lotStatus == null) return false;

                    return true;
                }).WithMessage("Lot status was not created in DB");


            RuleFor(l => l.Id)
                .NotEmpty()
                .MustAsync(async (id, cancellationToken) =>
                {
                    var lot = await _lotRepository.GetAsync(l => l.Id == id,
                        enableTracking: false,
                        cancellationToken: cancellationToken);


                    return lot == null ? false : true;
                })
                .WithMessage("Lot with indicated ID does not exist")
                .MustAsync(async (lotMain, id, context, cancellationToken) =>
                {
                    var lot = await _lotRepository.GetAsync(l => l.Id == id,
                        include: x => x.Include(l => l.LotStatus),
                        enableTracking: false,
                        cancellationToken: cancellationToken);
                    
                    if (lot == null) return false;

                    if (lot.LotStatus.Name == AuctionStatus.Draft.ToString() ||
                        lot.LotStatus.Name == AuctionStatus.PendingApproval.ToString() ||
                        lot.LotStatus.Name == AuctionStatus.Upcoming.ToString())
                    {
                        return true;
                    }
                    context.AddFailure("Lot's status does not allow edit");
                    return false;
                });

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
                .Unless(l => l.StartingPrice == null)
                .WithMessage("Starting price has to be greater than 0");

            RuleFor(l => l.CategoryId)
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    var category = await _categoryRepository.GetAsync(c => c.Id == categoryId, cancellationToken: cancellationToken);
                    if (category == null) return false;

                    return true;
                })
                .Unless(l => l.CategoryId == null)
                .WithMessage("The category entered does not exist in the system");

            RuleFor(l => l.CurrencyId)
                .MustAsync(async (currencyId, cancellationToken) =>
                {
                    var currency = await _currencyRepository.GetAsync(c => c.Id == currencyId, cancellationToken: cancellationToken);
                    if (currency == null) return false;

                    return true;
                })
                .Unless(l => l.CurrencyId == null)
                .WithMessage("Currency entered does not exist in the system");

            ConfigureValidationWhenConcreteUpdating();
        }

        private void ConfigureValidationWhenConcreteUpdating()
        {
            //RuleFor(l => l.Photos)
            //    .NotEmpty()
            //    .When(l => !l.IsDraft);

            RuleFor(l => l.City)
                .NotEmpty()
                .When(l => !l.IsDraft);

            RuleFor(l => l.Country)
                .NotEmpty()
                .When(l => !l.IsDraft);

            RuleFor(l => l.Address)
                .NotEmpty()
                .When(l => !l.IsDraft);

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
                .Must((lot, time) =>
                {
                    var timeDifference = time - lot.StartDate;
                    if (timeDifference.TotalHours >= minimumEndDateAndStartDateDiff) return true;

                    return false;
                })
                .When(l => !l.IsDraft)
                .When(l => l.EndDate >= DateTime.MinValue)
                .WithMessage("End date has to be at least 4 hours ahead than start date");
        }
    }
}
