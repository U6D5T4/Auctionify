using Auctionify.Application.Features.Reports.Query.Generate;
using Auctionify.Application.Features.Reports.Query.GetAllLotsStatuses;
using Auctionify.Application.Features.Reports.Query.GetCreatedLotsCount;
using Auctionify.Application.Features.Reports.Query.GetUserIncome;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Auctionify.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ReportsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ReportsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[Authorize(Roles = "Seller")]
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

			return File(reportBytes, "application/octet-stream", $"Report For {monthsDuration} months.{reportType.ToString()}");
		}

		[HttpGet("analytics/created-count")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> GetCreatedCount([FromQuery] GetCreatedLotsCountQuery query, CancellationToken cancellationToken)
		{
			var result = await _mediator.Send(query, cancellationToken);

			return Ok(result);
		}

		[HttpGet("analytics/lots-statuses")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> GetLotsStatuses(CancellationToken cancellationToken)
		{
			var result = await _mediator.Send(new GetAllLotsStatusesQuery(), cancellationToken);

			return Ok(result);
		}

		[HttpGet("analytics/income")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> GetUserIncome([FromQuery] GetUserIncomeQuery query, CancellationToken cancellationToken)
		{
			var result = await _mediator.Send(query, cancellationToken);

			return Ok(result);
		}
	}
}
