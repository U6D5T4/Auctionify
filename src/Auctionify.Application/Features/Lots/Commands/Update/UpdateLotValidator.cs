using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotValidator : AbstractValidator<UpdateLotCommand>
    {
        public UpdateLotValidator()
        {
        }
    }
}
