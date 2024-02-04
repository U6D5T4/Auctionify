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

		public async Task JoinConversationGroup(int conversationId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
		}

		public async Task LeaveConversationGroup(int conversationId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
		}
	}
}
