using Auctionify.Application.Common.DTOs;

namespace Auctionify.Application.Features.Users.Queries.GetById
{
    public class GetByIdUserResponse
    {
        public string Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public ICollection<LotDto> SellingLots { get; set; }

        public ICollection<RateDto> ReceiverRates { get; set; }
    }
}
