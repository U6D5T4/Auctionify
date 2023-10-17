using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
    public class LotStatusRepository : EfBaseRepository<LotStatus, ApplicationDbContext>, ILotStatusRepository
    {
        public LotStatusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
