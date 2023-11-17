using Auctionify.Application.Features.Locations.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LocationsController : Controller
	{
		private readonly IMediator _mediator;

		public LocationsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IList<GetAllLocationsResponse>>> GetAll()
		{
			var result = await _mediator.Send(new GetAllLocationsQuery());
			return Ok(result);
		}
	}
}
