using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.Create
{
    public class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
    {

        public CreateLotCommandValidator(ICategoryRepository categoryRepository,
            ILotStatusRepository lotStatusRepository,
            ICurrencyRepository currencyRepository)
        {

            Include(new BaseLotCommandsValidator(
                categoryRepository,
                lotStatusRepository,
                currencyRepository));
        }
    }
}
