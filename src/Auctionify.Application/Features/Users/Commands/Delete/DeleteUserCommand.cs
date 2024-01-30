﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Commands.Delete
{
	public class DeleteUserCommand : IRequest<DeletedUserResponse> { }

	public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeletedUserResponse>
	{
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public DeleteUserCommandHandler(
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_userManager = userManager;
			_currentUserService = currentUserService;
		}

		public async Task<DeletedUserResponse> Handle(
			DeleteUserCommand request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			user!.IsDeleted = true;

			user!.DeletionDate = DateTime.UtcNow;

			await _userManager.UpdateAsync(user);

			return new DeletedUserResponse
			{
				IsDeleted = true,
				Message = "User deleted successfully"
			};
		}
	}
}