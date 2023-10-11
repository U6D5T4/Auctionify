namespace Auctionify.Application.Common.Models.BaseModel
{
    public abstract class BaseResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }

    }
}
