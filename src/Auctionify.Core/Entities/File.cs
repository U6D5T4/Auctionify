using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class File : BaseAuditableEntity
    {
        public int LotId { get; set; }

        public Lot Lot { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }
    }
}
