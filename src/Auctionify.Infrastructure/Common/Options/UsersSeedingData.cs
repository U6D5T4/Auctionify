namespace Auctionify.Infrastructure.Common.Options
{
    public class UsersSeedingData
    {
        public UserEmails Emails { get; set; }

        public string Password { get; set; }
    }

    public class UserEmails
    {
        public string Admin { get; set; }

        public string Buyer { get; set; }

        public string Seller { get; set; }
    }
}
