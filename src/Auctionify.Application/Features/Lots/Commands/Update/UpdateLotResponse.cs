using Auctionify.Application.Common.DTOs;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CategoryDto Category { get; set; }

        public LotStatusDto LotStatus { get; set; }

        public LocationDto Location { get; set; }

        public CurrencyDto Currency { get; set; }

        public ICollection<BidDto> Bids { get; set; }
    }
}
