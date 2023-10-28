using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Application.Features.Lots.Commands.DeleteLotFile;
using Auctionify.Application.Features.Lots.Commands.Update;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Application.Features.Lots.Queries.GetAllByName;
using Auctionify.Application.Features.Lots.Queries.GetById;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
		[Authorize(Roles = "Seller")]
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
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetAll([FromQuery] PageRequest pageRequest)
		{
			var query = new GetAllLotsQuery { PageRequest = pageRequest };
			var lots = await _mediator.Send(query);

			return Ok(lots);
		}
    
        [HttpGet("{location}")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetLotsByCity([FromRoute] string location, [FromQuery] PageRequest pageRequest)
        {
            var query = new GetAllLotsByLocationQuery { Location = location, PageRequest = pageRequest };
            var lots = await _mediator.Send(query);

            return Ok(lots);
        }

		[HttpGet("{name}")]
		[Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetLotsByName([FromQuery] string name, [FromQuery] PageRequest pageRequest)
        {
			var query = new GetAllLotsByNameQuery { Name = name, PageRequest = pageRequest };
            var lots = await _mediator.Send(query);

			return Ok(lots);
		}

		[HttpPut("{id}/statuses")]
		public async Task<IActionResult> UpdateLotStatus([FromRoute] int id, [FromQuery] AuctionStatus status)
		{
			var result = await _mediator.Send(new UpdateLotStatusCommand { Id = id, Name = status.ToString() });

			return Ok("Successfully updated lot status of lot with id: " + result.Id + " to " + status.ToString());
		}

		[HttpDelete("{id}/files")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> DeleteLotFile([FromRoute] int id, [FromBody] List<string> url)
		{
			var result = await _mediator.Send(new DeleteLotFileCommand { LotId = id, FileUrl = url });

			return Ok(result.WasDeleted
				? $"Successfully deleted specified files of lot with id: {result.LotId}"
				: $"Could not delete specified files of lot with id: {result.LotId}");
		}
	}
}
