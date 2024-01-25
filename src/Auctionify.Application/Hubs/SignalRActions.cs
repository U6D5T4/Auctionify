namespace Auctionify.Application.Hubs
{
	public static class SignalRActions
	{
		public const string ReceiveBidNotification = "ReceiveBidNotification";
		public const string ReceiveWithdrawBidNotification = "ReceiveWithdrawBidNotification";

		public const string ReceiveChatMessageNotification = "ReceiveChatMessageNotification";
		public const string ReceiveChatMessageReadNotification =
			"ReceiveChatMessageReadNotification";
	}
}
