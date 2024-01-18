using Auctionify.Application.Features.Chats.Commands.CreateChatMessage;
using Auctionify.Application.Features.Chats.Commands.CreateConversation;
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

		[HttpPost("users/conversations")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> CreateConversation([FromForm] int lotId)
		{
			var createConversationCommand = new CreateConversationCommand { LotId = lotId };

			var result = await _mediator.Send(createConversationCommand);

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

		[HttpPost("users/conversations/{conversationId}/messages")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> CreateChatMessage(
			[FromRoute] int conversationId,
			[FromForm] string body
		)
		{
			var createChatMessageCommand = new CreateChatMessageCommand
			{
				ConversationId = conversationId,
				Body = body
			};

			_ = await _mediator.Send(createChatMessageCommand);

			return Ok("Message sent successfully!");
		}
	}
}
