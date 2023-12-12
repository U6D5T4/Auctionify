using Auctionify.Application.Common.Interfaces.Repositories;
using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.UpdateLotStatus
{
	public class UpdateLotStatusCommandValidator : AbstractValidator<UpdateLotStatusCommand>
	{
		private readonly ILotStatusRepository _lotStatusRepository;

		public UpdateLotStatusCommandValidator(ILotStatusRepository lotStatusRepository)
		{
			_lotStatusRepository = lotStatusRepository;

			RuleFor(x => x.Name)
				.MustAsync(
					async (name, cancellationToken) =>
					{
						var lotStatus = await _lotStatusRepository.GetAsync(
							predicate: x => x.Name == name,
							cancellationToken: cancellationToken
						);

						return lotStatus != null;
					}
				)
				.WithMessage("Lot status with this name does not exist");
		}
	}
}
