using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Update;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Application.Features.Lots.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auctionify.Application.Features.Lots.Queries.GetAllByName;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LotsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public LotsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> Create([FromForm] CreateLotCommand createLotCommand)
		{
			var result = await _mediator.Send(createLotCommand);

			return Ok(result);
		}

		[HttpPut]
		[Authorize(Roles = "Seller")]
        public async Task<IActionResult> Update([FromForm] UpdateLotCommand updateLotCommand)
        {
            var result = await _mediator.Send(updateLotCommand);

            return Ok(result);
        }

        [HttpGet("{id}")]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			var result = await _mediator.Send(new GetByIdLotQuery { Id = id });

			return Ok(result);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var result = await _mediator.Send(new DeleteLotCommand { Id = id });

			return Ok($"Successfully deleted lot with id: {result.Id}");
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var query = new GetAllLotsQuery();
			var lots = await _mediator.Send(query);

			return Ok(lots);
		}

        [HttpGet("[action]/{location}")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetLotsByLocation([FromRoute] string location)
        {
            var query = new GetAllLotsByLocationQuery { Location = location };
            var lots = await _mediator.Send(query);

            return Ok(lots);
        }
    }
}
