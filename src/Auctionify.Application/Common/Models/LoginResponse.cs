namespace Auctionify.Application.Common.Models
{
    /// <summary>
    /// Response model for login flow
    /// </summary>
    public class LoginResponse
    {
        public bool Succeeded  { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; set; }
    }
}
