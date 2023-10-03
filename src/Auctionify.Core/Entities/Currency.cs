using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class Currency : BaseAuditableEntity
    {
        public string Code { get; set; }
    }
}
