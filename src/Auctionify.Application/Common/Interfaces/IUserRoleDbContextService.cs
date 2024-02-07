using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Auctionify.Application.Common.Interfaces
{
	public interface IUserRoleDbContextService
	{
		Task<UserRole> GetAsync(
			Expression<Func<UserRole, bool>> predicate,
			Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		);

		Task<UserRole> AddAsync(UserRole entity);

		Task<UserRole> DeleteAsync(UserRole entity, bool permanent = false);

		Task<UserRole> UpdateAsync(UserRole entity);

		Task<bool> AllAsync(
			Expression<Func<UserRole, bool>> predicate,
			CancellationToken cancellationToken = default
		);

		Task<bool> AnyAsync(
			Expression<Func<UserRole, bool>> predicate,
			CancellationToken cancellationToken = default
		);

		Task<ICollection<UserRole>> GetUnpaginatedListAsync(
			Expression<Func<UserRole, bool>>? predicate = null,
			Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
			Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		);

		Task<int> CountAsync(
			Expression<Func<UserRole, bool>>? predicate = null,
			CancellationToken cancellationToken = default
		);
	}
}
