using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.DTOs
{
    public class RateDto
    {
        public int Id { get; set; }

        public int ReceiverId { get; set; }

        public UserDto Receiver { get; set; }

        public int SenderId { get; set; }

        public UserDto Sender { get; set; }

        public byte RatingValue { get; set; }

        public string Comment { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
