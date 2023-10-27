using Auctionify.Application.Features.Categories.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IList<GetAllCateogoriesResponse>>> GetAll()
        {
            var resul = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(resul);
        }
    }
}
