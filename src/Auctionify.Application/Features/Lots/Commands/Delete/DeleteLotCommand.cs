using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Lots.Commands.Delete
{
	public class DeleteLotCommand : IRequest<DeletedLotResponse>
	{
		public int Id { get; set; }
		
		public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand, DeletedLotResponse>
		{
			private readonly ILotRepository _lotRepository;
			private readonly IMapper _mapper;

			public DeleteLotCommandHandler(ILotRepository lotRepository, IMapper mapper)
			{
				_lotRepository = lotRepository;
				_mapper = mapper;
			}

			public async Task<DeletedLotResponse> Handle(DeleteLotCommand request, CancellationToken cancellationToken)
			{
				var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);

				await _lotRepository.DeleteAsync(lot!);

				var response = _mapper.Map<DeletedLotResponse>(lot);

				return response;
			}
		}

	}
}
