using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
	public class UpdateLotCommandValidator : AbstractValidator<UpdateLotCommand>
	{
		private readonly ILotRepository _lotRepository;

		public UpdateLotCommandValidator(ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			ICategoryRepository categoryRepository,
			ICurrencyRepository currencyRepository,
			IFileRepository fileRepository)
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

			RuleFor(l => l.Photos)
				.MustAsync(async (cmd, photos, cancellationToken) =>
				{
					var pattern = new Regex(@"photos\/");

					var dbPhotos = await fileRepository.GetListAsync(predicate: x => x.LotId == cmd.Id, size: 100);

					foreach (var item in dbPhotos.Items)
					{
						var result = pattern.Match(item.Path);

						if (result.Success || photos.Count > 0)
						{
							return true;
						}
					}

					return false;
				})
				.When(l => !l.IsDraft);
		}
	}
}
