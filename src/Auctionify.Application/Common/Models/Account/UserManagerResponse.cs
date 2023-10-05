namespace Auctionify.Application.Common.Models.Account
{
    public class UserManagerResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public object Result { get; set; }
    }
}
