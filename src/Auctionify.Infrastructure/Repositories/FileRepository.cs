using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class FileRepository : EfBaseRepository<Core.Entities.File, ApplicationDbContext>, IFileRepository
	{
		public FileRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
