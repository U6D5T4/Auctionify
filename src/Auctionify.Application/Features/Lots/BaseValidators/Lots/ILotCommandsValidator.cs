using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Features.Lots.BaseValidators.Lots
{
    public interface ILotCommandsValidator
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }

        public string City { get; set; }

        public string? State { get; set; }

        public string Country { get; set; }

        public string Address { get; set; }

        public int? CurrencyId { get; set; }

        public IList<IFormFile>? Photos { get; set; }

        public IList<IFormFile>? AdditionalDocuments { get; set; }

        public bool IsDraft { get; set; }
    }
}
