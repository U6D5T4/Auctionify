using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Hubs
{
	[Authorize]
	public class AuctionHub : Hub
	{
		public async Task JoinLotGroup(int lotId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, lotId.ToString());
		}

		public async Task SendBidNotification(int lotId)
		{
			await Clients.Group(lotId.ToString()).SendAsync("ReceiveBidNotification");
		}

		public async Task SendWithdrawBidNotification(int lotId)
		{
			await Clients.Group(lotId.ToString()).SendAsync("ReceiveWithdrawBidNotification");
		}
	}
}
