using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;

namespace Auctionify.Application.Common.Interfaces.Repositories
{
	public interface ILocationRepository : IAsyncRepository<Location>
	{
	}
}
