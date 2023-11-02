using Auctionify.Application.Common.Models.BaseModels;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Models.Account
{
    public class AssignRoleToUserResponse : BaseResponse
    {
        public Role Role { get; set; }
    }
}
