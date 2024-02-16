using Auctionify.Application.Features.Reports.Query.Generate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ReportsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ReportsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[Authorize(Roles ="Seller")]
		public async Task<IActionResult> GetReport([FromBody]int monthsDuration, ReportType reportType)
		{
			var query = new GenerateReportQuery
			{
				MonthsDuration = monthsDuration,
				Format = reportType
			};

			var reportBytes = await _mediator.Send(query);

			return File(reportBytes, "application/octet-stream", $"Report.For{monthsDuration}.{reportType.ToString()}");
		}
	}
}
