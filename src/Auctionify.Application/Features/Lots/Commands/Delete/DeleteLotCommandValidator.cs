using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.Delete
{
	public class DeleteLotCommandValidator : AbstractValidator<DeleteLotCommand>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public DeleteLotCommandValidator(
			ILotRepository lotRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;

			RuleFor(x => x.Id)
				.MustAsync(
					async (id, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == id,
							cancellationToken: cancellationToken
						);

						return lot != null;
					}
				)
				.WithMessage("Lot with this Id does not exist");

			RuleFor(x => x.Id)
				.MustAsync(
					async (id, cancellationToken) =>
					{
						var user = await _userManager.Users.FirstOrDefaultAsync(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
							cancellationToken: cancellationToken
						);

						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == id,
							cancellationToken: cancellationToken
						);

						return lot!.SellerId == user!.Id;
					}
				)
				.WithMessage("You can delete only your own lot");
		}
	}
}
