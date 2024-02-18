using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
    public class SubscriptionRepository : EfBaseRepository<Subscription, ApplicationDbContext>, ISubscriptionRepository
    {
        public SubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
