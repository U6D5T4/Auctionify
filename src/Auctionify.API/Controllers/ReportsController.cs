using Auctionify.Application.Features.Reports.Query.Generate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
		public async Task<IActionResult> GetReport(
			[FromQuery][Required] int monthsDuration, 
			[FromQuery] ReportType reportType = ReportType.PDF
		)
		{
			if (monthsDuration < 1)
			{
				return BadRequest("Months duration must be greater than 0");
			}

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
