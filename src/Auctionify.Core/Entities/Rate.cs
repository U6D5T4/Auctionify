using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    public class Rate : BaseAuditableEntity
    {
        public int URate { get; set; }
        public string Comment { get; set; }
    }
}
