using Auctionify.Application.Features.Currencies.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CurrenciesController : Controller
	{
		private readonly IMediator _mediator;

		public CurrenciesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		public async Task<ActionResult<IList<GetAllCurrenciesResponse>>> GetAll()
		{
			var result = await _mediator.Send(new GetAllCurrenciesQuery());
			return Ok(result);
		}
	}
}
