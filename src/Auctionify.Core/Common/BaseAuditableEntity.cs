using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Common
{
    internal class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
