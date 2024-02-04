using Auctionify.Application.Common.Interfaces.Repositories;
using MediatR;

namespace Auctionify.Application.Features.Chats.Commands.MarkChatMessageAsRead
{
	public class MarkChatMessageAsReadCommand : IRequest<MarkedChatMessageAsReadResponse>
	{
		public int ChatMessageId { get; set; }
	}

	public class MarkChatMessageAsReadCommandHandler
		: IRequestHandler<MarkChatMessageAsReadCommand, MarkedChatMessageAsReadResponse>
	{
		private readonly IChatMessageRepository _chatMessageRepository;

		public MarkChatMessageAsReadCommandHandler(IChatMessageRepository chatMessageRepository)
		{
			_chatMessageRepository = chatMessageRepository;
		}

		public async Task<MarkedChatMessageAsReadResponse> Handle(
			MarkChatMessageAsReadCommand request,
			CancellationToken cancellationToken
		)
		{
			var chatMessage = await _chatMessageRepository.GetAsync(
				predicate: x => x.Id == request.ChatMessageId,
				cancellationToken: cancellationToken
			);

			chatMessage!.IsRead = true;

			await _chatMessageRepository.UpdateAsync(chatMessage);

			return new MarkedChatMessageAsReadResponse
			{
				ChatMessageId = chatMessage.Id,
				Success = true,
				Message = "Chat message marked as read."
			};
		}
	}
}
