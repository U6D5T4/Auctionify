using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
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
		private readonly IUserRoleDbContextService _userRoleDbContextService;
		private readonly RoleManager<Role> _roleManager;

		public DeleteUserCommandHandler(
			UserManager<User> userManager,
			ICurrentUserService currentUserService,
			IUserRoleDbContextService userRoleDbContextService,
			RoleManager<Role> roleManager
		)
		{
			_userManager = userManager;
			_currentUserService = currentUserService;
			_userRoleDbContextService = userRoleDbContextService;
			_roleManager = roleManager;
		}

		public async Task<DeletedUserResponse> Handle(
			DeleteUserCommand request,
			CancellationToken cancellationToken
		)
		{
			var currentUserEmail = _currentUserService.UserEmail!;
			var currentUserRole = _currentUserService.UserRole!;
			var currentUserRoleId = (await _roleManager.FindByNameAsync(currentUserRole))!.Id;

			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == currentUserEmail && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var userRole = await _userRoleDbContextService.GetAsync(
				ur => ur.UserId == user!.Id && ur.RoleId == currentUserRoleId && !ur.IsDeleted,
				cancellationToken: cancellationToken
			);

			if (userRole == null)
			{
				return new DeletedUserResponse
				{
					IsDeleted = false,
					Message = "User has been deleted before"
				};
			}

			var userRolesCount = await _userRoleDbContextService.CountAsync(
				ur => ur.UserId == user!.Id && !ur.IsDeleted,
				cancellationToken
			);

			if (userRolesCount == 1)
			{
				user!.IsDeleted = true;
				user!.DeletionDate = DateTime.UtcNow;
				await _userManager.UpdateAsync(user);

				userRole!.IsDeleted = true;
				userRole!.DeletionDate = DateTime.UtcNow;
				await _userRoleDbContextService.UpdateAsync(userRole);
			}
			else // which is 2 (buyer and seller)
			{
				userRole!.IsDeleted = true;
				userRole!.DeletionDate = DateTime.UtcNow;
				await _userRoleDbContextService.UpdateAsync(userRole);
			}

			return new DeletedUserResponse
			{
				IsDeleted = true,
				Message = "User deleted successfully"
			};
		}
	}
}
