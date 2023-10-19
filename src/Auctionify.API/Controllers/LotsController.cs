using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Application.Features.Lots.Queries.GetAllByName;
using Auctionify.Application.Features.Lots.Queries.GetById;
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

		[HttpGet("[action]/{name}")]
		[Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetLotsByName([FromRoute] string name)
        {
			var query = new GetAllLotsByNameQuery { Name = name };
            var lots = await _mediator.Send(query);

            return Ok(lots);
        }
    }
}
