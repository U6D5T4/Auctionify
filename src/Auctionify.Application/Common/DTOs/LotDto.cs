using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.DTOs
{
    public class LotDto
    {
        public int Id { get; set; }

        public int LotStatusId { get; set; }

        public virtual LotStatus LotStatus { get; set; }

        public string? Title { get; set; }

        public decimal? StartingPrice { get; set; }

        public virtual DateTime EndDate { get; set; }
    }
}
