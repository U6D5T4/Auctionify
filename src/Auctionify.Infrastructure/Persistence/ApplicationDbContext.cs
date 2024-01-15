using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Common;
using Auctionify.Infrastructure.Interceptors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Auctionify.Infrastructure.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<User, Role, int>
	{
		private readonly IMediator? _mediator;
		private readonly AuditableEntitySaveChangesInterceptor? _auditableEntitiesInterceptor;

		public ApplicationDbContext() { }

		public ApplicationDbContext(DbContextOptions options,
			AuditableEntitySaveChangesInterceptor auditableEntitiesInterceptor,
			IMediator mediator) : base(options)
		{
			_auditableEntitiesInterceptor = auditableEntitiesInterceptor;
			_mediator = mediator;
		}

		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		public virtual DbSet<Category> Categories => Set<Category>();

		public virtual DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

		public virtual DbSet<Lot> Lots => Set<Lot>();

		public virtual DbSet<Rate> Rates => Set<Rate>();

		public virtual DbSet<Watchlist> Watchlists => Set<Watchlist>();

		public virtual DbSet<Bid> Bids => Set<Bid>();

		public virtual DbSet<Currency> Currency => Set<Currency>();

		public virtual DbSet<Location> Locations => Set<Location>();

		public virtual DbSet<LotStatus> LotStatuses => Set<LotStatus>();

		public virtual DbSet<Core.Entities.File> Files => Set<Core.Entities.File>();

		public virtual DbSet<Subscription> Subscriptions => Set<Subscription>();

		public virtual DbSet<SubscriptionType> SubscriptionTypes => Set<SubscriptionType>();

		public virtual DbSet<Conversation> Conversations => Set<Conversation>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>()
				.Ignore(u => u.AccessFailedCount)
				.Ignore(u => u.LockoutEnabled)
				.Ignore(u => u.LockoutEnd)
				.Ignore(u => u.TwoFactorEnabled)
				.Ignore(u => u.PhoneNumberConfirmed);

			builder.Entity<User>().ToTable("Users");
			builder.Entity<Role>().ToTable("Roles");
			builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
			builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
			builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
			builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
			builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.AddInterceptors(_auditableEntitiesInterceptor);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await _mediator.DispatchDomainEvents(this);

			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}
