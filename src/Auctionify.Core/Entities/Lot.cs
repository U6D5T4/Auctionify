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
    internal class Lot : BaseAuditableEntity
    {
        public User SellerId { get; set; }

        public User? BuyerId { get; set; }

        public Category? CategoryId { get; set; }

        public LotStatus Status { get; set; }

        public Location? Location { get; set; }

        public Currency? Currency { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
