using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Auctionify.Infrastructure.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<User, Role, int>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

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
			builder.Ignore<IdentityUserLogin<int>>();
			builder.Ignore<IdentityUserToken<int>>();

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

			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
