namespace Auctionify.Core.Common
{
	public class BaseAuditableEntity : BaseEntity
	{
		public DateTime CreationDate { get; set; }
		public DateTime ModificationDate { get; set; }
	}
}
