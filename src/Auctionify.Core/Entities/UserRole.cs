using Microsoft.AspNetCore.Identity;

namespace Auctionify.Core.Entities
{
	public class UserRole : IdentityUserRole<int>
	{
		public DateTime CreationDate { get; set; }

		public DateTime DeletionDate { get; set; }

		public bool IsDeleted { get; set; }
	}
}
