namespace Auctionify.Application.Common.Models.Account
{
    public class UserManagerResponse // This is the model that the server will send back to the user
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public object Result { get; set; }
    }
}
