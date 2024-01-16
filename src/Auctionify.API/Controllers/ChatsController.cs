using Auctionify.Application.Features.Chats.Queries.GetAllChatMessages;
using Auctionify.Application.Features.Chats.Queries.GetAllUserConversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ChatsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("users/conversations")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetAllUserConversations()
		{
			var result = await _mediator.Send(new GetAllUserConversationsQuery());

			return Ok(result);
		}

		[HttpGet("users/conversations/{conversationId}/messages")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetAllConversationChatMessages(int conversationId)
		{
			var result = await _mediator.Send(
				new GetAllChatMessagesQuery { ConversationId = conversationId }
			);

			return Ok(result);
		}
	}
}
