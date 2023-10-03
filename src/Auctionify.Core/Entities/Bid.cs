using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class Bid : BaseAuditableEntity
    {
        public int Id { get; set; }

        public User BuyerId { get; set; }

        public decimal NewPrice { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
