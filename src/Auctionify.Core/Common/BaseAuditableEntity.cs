using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
