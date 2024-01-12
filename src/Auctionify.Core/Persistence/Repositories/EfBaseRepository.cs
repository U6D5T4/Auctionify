using Auctionify.Core.Common;
using Auctionify.Core.Persistence.Dynamic;
using Auctionify.Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Auctionify.Core.Persistence.Repositories
{
	public class EfBaseRepository<TEntity, TContext> : IAsyncRepository<TEntity>
		where TEntity : BaseEntity
		where TContext : DbContext
	{
		protected readonly TContext _context;

		public EfBaseRepository(TContext context)
		{
			_context = context;
		}

		public IQueryable<TEntity> Query()
		{
			return _context.Set<TEntity>();
		}

		public async Task<TEntity> AddAsync(TEntity entity)
		{
			await _context.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
		{
			await _context.AddRangeAsync(entities);
			await _context.SaveChangesAsync();
			return entities;
		}

		public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
		{
			_context.Remove(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<ICollection<TEntity>> DeleteRangeAsync(
			ICollection<TEntity> entities,
			bool permanent = false
		)
		{
			_context.RemoveRange(entities);
			await _context.SaveChangesAsync();
			return entities;
		}

		public async Task<TEntity?> GetAsync(
			Expression<Func<TEntity, bool>> predicate,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<TEntity> queryable = Query();
			if (!enableTracking)
				queryable = queryable.AsNoTracking();
			if (include != null)
				queryable = include(queryable);
			if (withDeleted)
				queryable = queryable.IgnoreQueryFilters();
			return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public async Task<IPaginate<TEntity>> GetListAsync(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
			int index = 0,
			int size = 10,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<TEntity> queryable = Query();
			if (!enableTracking)
				queryable = queryable.AsNoTracking();
			if (include != null)
				queryable = include(queryable);
			if (withDeleted)
				queryable = queryable.IgnoreQueryFilters();
			if (predicate != null)
				queryable = queryable.Where(predicate);
			if (orderBy != null)
				return await orderBy(queryable)
					.ToPaginateAsync(index, size, from: 0, cancellationToken: cancellationToken);
			return await queryable.ToPaginateAsync(
				index,
				size,
				from: 0,
				cancellationToken: cancellationToken
			);
		}

		public async Task<IPaginate<TEntity>> GetListByDynamicAsync(
			DynamicQuery dynamic,
			IQueryable<TEntity>? existingQueryable = null,
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
			int index = 0,
			int size = 10,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<TEntity> queryable = existingQueryable ?? Query();

			queryable = queryable.ToDynamic(dynamic);
			if (!enableTracking)
				queryable = queryable.AsNoTracking();
			if (include != null)
				queryable = include(queryable);
			if (withDeleted)
				queryable = queryable.IgnoreQueryFilters();
			if (predicate != null)
				queryable = queryable.Where(predicate);

			return await queryable.ToPaginateAsync(index, size, from: 0, cancellationToken);
		}

		public async Task<TEntity> UpdateAsync(TEntity entity)
		{
			_context.Update(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
		{
			_context.UpdateRange(entities);
			await _context.SaveChangesAsync();
			return entities;
		}

		public async Task<ICollection<TEntity>> GetUnpaginatedListAsync(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
			bool withDeleted = false,
			bool enableTracking = true,
			CancellationToken cancellationToken = default
		)
		{
			IQueryable<TEntity> queryable = Query();
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
	}
}
