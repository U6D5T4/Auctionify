using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.BaseValidators.Lots;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotCommand : IRequest<UpdateLotResponse>, ILotCommandsValidator
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }

        public string City { get; set; }

        public string? State { get; set; }

        public string Country { get; set; }

        public string Address { get; set; }

        public int? CurrencyId { get; set; }

        public IList<IFormFile>? Photos { get; set; }

        public IList<IFormFile>? AdditionalDocuments { get; set; }

        public bool IsDraft { get; set; }

    }

    public class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand, UpdateLotResponse>
    {
        private readonly ILotRepository _lotRepository;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly IMapper _mapper;

        public UpdateLotCommandHandler(ILotRepository lotRepository,
            ILotStatusRepository lotStatusRepository,
            IMapper mapper)
        {
            _lotRepository = lotRepository;
            _lotStatusRepository = lotStatusRepository;
            _mapper = mapper;
        }

        public async Task<UpdateLotResponse> Handle(UpdateLotCommand request, CancellationToken cancellationToken)
        {
            AuctionStatus status = request.IsDraft ? AuctionStatus.Draft : AuctionStatus.Upcoming;

            var lotStatus = await _lotStatusRepository.GetAsync(s => s.Name == status.ToString());


            var lot = await _lotRepository.GetAsync(l => l.Id == request.Id,
                include: x => x.Include(l => l.Location),
                cancellationToken: cancellationToken,
                enableTracking: false);

            lot.LotStatus = lotStatus;

            var lotUpdated = _mapper.Map(request, lot);

            await _lotRepository.UpdateAsync(lotUpdated);

            return _mapper.Map<UpdateLotResponse>(lotUpdated);

        }
    }
}
