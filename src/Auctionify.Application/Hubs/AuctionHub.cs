using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Hubs
{
	public class AuctionHub : Hub
	{
		public async Task SendBidNotification()
		{
			await Clients.All.SendAsync("ReceiveBidNotification");
		}

		public async Task SendWithdrawBidNotification()
		{
			await Clients.All.SendAsync("ReceiveWithdrawBidNotification");
		}

	}
}
