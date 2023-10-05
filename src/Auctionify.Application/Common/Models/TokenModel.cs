namespace Auctionify.Application.Common.Models
{
    /// <summary>
    /// Token model
    /// </summary>
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
