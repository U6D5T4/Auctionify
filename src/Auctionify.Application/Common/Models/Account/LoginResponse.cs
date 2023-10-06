using Auctionify.Application.Common.Models.BaseModels;

namespace Auctionify.Application.Common.Models.Account
{
    public class LoginResponse: BaseResponse
    {
        public TokenModel Result { get; set; }
    }
}
