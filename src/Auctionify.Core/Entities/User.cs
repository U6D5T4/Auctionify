using Microsoft.AspNetCore.Identity;

namespace Auctionify.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

    }
}
