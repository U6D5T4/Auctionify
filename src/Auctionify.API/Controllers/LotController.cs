using Auctionify.Application.Features.Lots.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class LotController : ControllerBase
	{
		private readonly IMediator mediator;

		public LotController(IMediator mediator)
		{
			this.mediator = mediator;
		}

		[HttpPost]
		public async Task<IActionResult> CreateLot(CreateLotCommand createLotCommand)
		{
			var result = await mediator.Send(createLotCommand);

			return Ok(result);
		}
	}
}
