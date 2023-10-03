using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class SubscriptionType : BaseAuditableEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
