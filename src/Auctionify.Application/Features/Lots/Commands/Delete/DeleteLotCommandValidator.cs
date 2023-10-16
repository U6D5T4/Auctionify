using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.Delete
{
	public class DeleteLotCommandValidator : AbstractValidator<DeleteLotCommand>
	{
		private readonly ILotRepository _lotRepository;

		public DeleteLotCommandValidator(ILotRepository lotRepository, ILotStatusRepository lotStatusRepository)
		{
			_lotRepository = lotRepository;

			RuleFor(x => x.Id)
				.GreaterThan(0)
				.WithMessage("Id must be greater than 0");

			RuleFor(x => x.Id)
				.MustAsync(async (id, cancellationToken) =>
				{
					var lot = await _lotRepository.GetAsync(predicate: x => x.Id == id, cancellationToken: cancellationToken);

					return lot != null;
				})
				.WithMessage("Lot with this Id does not exist");
		}
	}
}
