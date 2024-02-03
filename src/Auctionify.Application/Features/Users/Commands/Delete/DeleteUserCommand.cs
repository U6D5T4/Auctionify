using Auctionify.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User = Auctionify.Core.Entities.User;

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
			var currentUserEmail = _currentUserService.UserEmail!;
			var currentUserRole = _currentUserService.UserRole!;

			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			// I need to get that user where the email is equal to the current user email and the user is not deleted
			// and the UserRoles is equal to the current user role
			// maybe I need to use joins
			// using joins to get the user where the email is equal to the current user email and the user is not deleted
			// and the UserRoles is equal to the current user role

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
