using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class Location : BaseAuditableEntity
    {
        public int Id { get ; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(150)]
        public string Address { get; set; }

    }
}
