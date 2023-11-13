using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Hubs
{
	public class AuctionHub : Hub
	{
		public async Task SendBidNotification()
		{
			await Clients.All.SendAsync("ReceiveBidNotification");
		}
	}
}
