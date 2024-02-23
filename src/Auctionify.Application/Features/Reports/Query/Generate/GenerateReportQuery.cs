using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Reports.Query.Generate;
using Auctionify.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Reports.Query.Generate
{
	public class GenerateReportQuery : IRequest<byte[]>
	{
		public int MonthsDuration { get; set; }
		public ReportType Format { get; set; }
	}
}

public class GenerateReportHandler : IRequestHandler<GenerateReportQuery, byte[]>
{
	private readonly IReportDataRepository _reportDataRepository;
	private readonly ICurrentUserService _currentUserService;
	private readonly UserManager<User> _userManager;
	private readonly IReportService _reportGeneratorService;

	public GenerateReportHandler(
		IReportDataRepository reportDataRepository,
		ICurrentUserService currentUserService,
		UserManager<User> userManager,
		IReportService reportGeneratorService
	)
	{
		_reportDataRepository = reportDataRepository;
		_currentUserService = currentUserService;
		_userManager = userManager;
		_reportGeneratorService = reportGeneratorService;
	}

	public async Task<byte[]> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(
			u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
			cancellationToken: cancellationToken
		);

		var reportData = await _reportDataRepository.GetReportDataAsync(
			new ReportRequest { MonthsDuration = request.MonthsDuration, UserId = user.Id }
			);

		return _reportGeneratorService.GenerateReport(reportData, user, request.Format);
	}
}
