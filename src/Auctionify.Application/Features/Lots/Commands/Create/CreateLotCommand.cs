using Auctionify.Core.Entities;
using MediatR;

namespace Auctionify.Application.Features.Lots.Commands.Create
{
    public class CreateLotCommand : IRequest<CreatedLotResponse>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CategoryId { get; set; }

        public Location Location { get; set; }

        public int CurrencyId { get; set; }

    }

    public class CreateLotCommandHandler : IRequestHandler<CreatedLotResponse>
    {
        private readonly 

        public CreateLotCommandHandler() { }

        public Task Handle(CreatedLotResponse request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
