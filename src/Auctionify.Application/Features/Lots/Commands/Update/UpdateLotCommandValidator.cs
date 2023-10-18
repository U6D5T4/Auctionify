using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotCommandValidator : AbstractValidator<UpdateLotCommand>
    {
        private readonly ILotRepository _lotRepository;

        public UpdateLotCommandValidator(ILotRepository lotRepository,
            ILotStatusRepository lotStatusRepository,
            ICategoryRepository categoryRepository,
            ICurrencyRepository currencyRepository)
        {
            _lotRepository = lotRepository;

            Include(new BaseLotCommandsValidator(
                categoryRepository,
                lotStatusRepository,
                currencyRepository));

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
                .DependentRules(() =>
                {
                    RuleFor(l => l.Id)
                    .MustAsync(async (lotMain, id, context, cancellationToken) =>
                    {
                        var lot = await _lotRepository.GetAsync(l => l.Id == id,
                            include: x => x.Include(l => l.LotStatus),
                            enableTracking: false,
                            cancellationToken: cancellationToken);

                        if (lot.LotStatus.Name == AuctionStatus.Draft.ToString() ||
                            lot.LotStatus.Name == AuctionStatus.PendingApproval.ToString() ||
                            lot.LotStatus.Name == AuctionStatus.Upcoming.ToString())
                        {
                            return true;
                        }
                        return false;
                    })
                    .WithMessage("Lot's status does not allow edit");
                });
        }
    }
}
