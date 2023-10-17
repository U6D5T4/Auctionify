using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
