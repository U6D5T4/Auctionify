using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class User : BaseAuditableEntity
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? NormalizedUserName { get; set; }

        public string SecurityStamp { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public int? PhoneNumber { get ; set; }
    }
}
