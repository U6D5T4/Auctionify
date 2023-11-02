using Auctionify.Core.Persistence.Repositories;

namespace Auctionify.Application.Common.Interfaces.Repositories
{
	public interface IFileRepository: IAsyncRepository<Core.Entities.File>
	{
	}
}
