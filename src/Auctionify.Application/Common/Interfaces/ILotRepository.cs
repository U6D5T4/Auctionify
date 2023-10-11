using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Application.Common.Interfaces
{
    public interface ILotRepository : IAsyncRepository<Lot>, IQuery<Lot>
    {
        
    }
}
