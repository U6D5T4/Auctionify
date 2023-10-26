using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Update;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Application.Features.Lots.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auctionify.Core.Enums;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;

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

			return Ok(result.WasDeleted 
				? $"Successfully deleted lot with id: {result.Id}" 
				: $"Could not delete lot with id: {result.Id} since its status is {AuctionStatus.Active.ToString()}," +
				  $" but successfully updated status to {AuctionStatus.Cancelled.ToString()} and deleted all related bids.");
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var lots = await _mediator.Send(new GetAllLotsQuery());

			return Ok(lots);
		}

		[HttpPut("{id}/status")]
		public async Task<IActionResult> UpdateLotStatus([FromRoute] int id, [FromQuery] AuctionStatus status)
		{
			var result = await _mediator.Send(new UpdateLotStatusCommand { Id = id, Name = status.ToString() });

			return Ok("Successfully updated lot status of lot with id: " + result.Id + " to " + status.ToString());
		}
	}
}
