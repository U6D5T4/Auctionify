using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.UpdateLotStatus
{
	public class UpdateLotStatusCommand: IRequest<UpdateLotStatusResponse>
	{
		public int Id { get; set; }

		public string? Name { get; set; }
	}

	public class UpdateLotStatusCommandHandler : IRequestHandler<UpdateLotStatusCommand, UpdateLotStatusResponse>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IMapper _mapper;

		public UpdateLotStatusCommandHandler(ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository, IMapper mapper)
		{
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_mapper = mapper;
		}

		public async Task<UpdateLotStatusResponse> Handle(UpdateLotStatusCommand request, CancellationToken cancellationToken)
		{


			var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, include: x => x.Include(x => x.LotStatus));

			var lotStatus = await _lotStatusRepository.GetAsync(predicate: s => s.Name == request.Name);

			lot.LotStatusId = lotStatus.Id;

			await _lotRepository.UpdateAsync(lot);
			
			var result = _mapper.Map<UpdateLotStatusResponse>(lot);

			return result;
		}
	}
}
