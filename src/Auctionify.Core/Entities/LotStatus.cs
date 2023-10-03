using Auctionify.Core.Common;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    public class LotStatus : BaseAuditableEntity
    {
        public string Name { get; set; }
    }
}
