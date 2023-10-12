using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.Create
{
    public class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
    {
        public CreateLotCommandValidator()
        {
            RuleFor(l => l.Title)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(64);

            RuleFor(l => l.Description)
                .NotEmpty()
                .MinimumLength(30)
                .MaximumLength(500);

            RuleFor(l => l.StartingPrice)
                .NotEmpty();

            RuleFor(l => l.Location)
                .NotEmpty();

            RuleFor(l => l.Photos)
                .NotEmpty();

            RuleFor(l => l.AdditionalDocuments);
        }
    }
}
