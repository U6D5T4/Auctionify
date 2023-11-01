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
        private readonly IMediator _mediator;
        private readonly AuditableEntitySaveChangesInterceptor _auditableEntitiesInterceptor;

        public ApplicationDbContext(DbContextOptions options,
            AuditableEntitySaveChangesInterceptor auditableEntitiesInterceptor,
            IMediator mediator) : base(options)
        {
            _auditableEntitiesInterceptor = auditableEntitiesInterceptor;
            _mediator = mediator;
        }

		public DbSet<Category> Categories => Set<Category>();
		public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
		public DbSet<Lot> Lots => Set<Lot>();
		public DbSet<Rate> Rates => Set<Rate>();
		public DbSet<Watchlist> Watchlists => Set<Watchlist>();
		public DbSet<Bid> Bids => Set<Bid>();
		public DbSet<Currency> Currency => Set<Currency>();
		public DbSet<Location> Locations => Set<Location>();
		public DbSet<LotStatus> LotStatuses => Set<LotStatus>();
		public DbSet<Core.Entities.File> Files => Set<Core.Entities.File>();
		public DbSet<Subscription> Subscriptions => Set<Subscription>();
		public DbSet<SubscriptionType> SubscriptionTypes => Set<SubscriptionType>();

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
