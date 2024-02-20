using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Auctionify.Infrastructure.Services
{
	public class UserRoleDbContextService : IUserRoleDbContextService
	{
		private readonly ApplicationDbContext _context;

		public UserRoleDbContextService(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<UserRole> Query()
		{
			return _context.Set<UserRole>();
		}

		public async Task<UserRole> GetAsync(
			Expression<Func<UserRole, bool>> predicate,
			Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<UserRole> queryable = Query();
			if (!enableTracking)
				queryable = queryable.AsNoTracking();
			if (include != null)
				queryable = include(queryable);
			if (withDeleted)
				queryable = queryable.IgnoreQueryFilters();
			return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public async Task<UserRole> AddAsync(UserRole entity)
		{
			await _context.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<UserRole> DeleteAsync(UserRole entity, bool permanent = false)
		{
			_context.Remove(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<UserRole> UpdateAsync(UserRole entity)
		{
			_context.Update(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<bool> AllAsync(
			Expression<Func<UserRole, bool>> predicate,
			CancellationToken cancellationToken = default
		)
		{
			return await Query().AllAsync(predicate, cancellationToken);
		}

		public async Task<bool> AnyAsync(
			Expression<Func<UserRole, bool>> predicate,
			CancellationToken cancellationToken = default
		)
		{
			return await Query().AnyAsync(predicate, cancellationToken);
		}

		public async Task<ICollection<UserRole>> GetUnpaginatedListAsync(
			Expression<Func<UserRole, bool>>? predicate = null,
			Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
			Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<UserRole> queryable = Query();
			if (!enableTracking)
				queryable = queryable.AsNoTracking();
			if (include != null)
				queryable = include(queryable);
			if (withDeleted)
				queryable = queryable.IgnoreQueryFilters();
			if (predicate != null)
				queryable = queryable.Where(predicate);
			if (orderBy != null)
				return await orderBy(queryable).ToListAsync(cancellationToken);
			return await queryable.ToListAsync(cancellationToken);
		}

		public async Task<int> CountAsync(
			Expression<Func<UserRole, bool>>? predicate = null,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<UserRole> queryable = Query();
			if (predicate != null)
				queryable = queryable.Where(predicate);
			return await queryable.CountAsync(cancellationToken);
		}
	}
}
