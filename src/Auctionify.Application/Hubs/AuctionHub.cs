using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Hubs
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class AuctionHub : Hub
	{
		public async Task JoinLotGroup(int lotId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, lotId.ToString());
		}

		public async Task LeaveLotGroup(int lotId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, lotId.ToString());
		}
	}
}
