﻿using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Location : BaseAuditableEntity
    {
        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Address { get; set; }

    }
}
