using Microsoft.AspNetCore.Identity;

namespace Auctionify.Core.Entities
{
    public class User : IdentityUser<int>
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

    }
}
