using Microsoft.AspNetCore.Identity;

namespace Auctionify.Core.Entities
{
	public class UserRole : IdentityUserRole<int>
	{
		public int CreationDate { get; set; }

		public int DeletionDate { get; set; }

		public bool IsDeleted { get; set; }
	}
}
