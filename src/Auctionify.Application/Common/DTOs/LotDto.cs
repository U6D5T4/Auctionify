namespace Auctionify.Application.Common.DTOs
{
	public class LotDto
	{
		public int Id { get; set; }

		public int? UserId { get; set; }

		public UserDto User { get; set; }

		public int? CategoryId { get; set; }

		public CategoryDto Category { get; set; }
		
		public int? LotStatusId { get; set; }
		
		public virtual LotStatusDto LotStatus { get; set; }

		public int? LocationId { get; set; }

		public virtual LocationDto Location { get; set; }
		
		public int? CurrencyId { get; set; }

		public virtual CurrencyDto Currency { get; set; }
		
		public string Title { get; set; }

		public string Description { get; set; }

		public decimal? StartingPrice { get; set; }
		
		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public virtual ICollection<BidDto> Bids { get; set; }

		public virtual ICollection<FileDto> Files { get; set; }
	}
}
