using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Features.Lots.Commands.Update
{
    public class UpdateLotCommand : IRequest<UpdateLotResponse>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public int? CurrencyId { get; set; }

        public IList<IFormFile>? Photos { get; set; }

        public IList<IFormFile>? AdditionalDocuments { get; set; }

        public bool IsDraft { get; set; }

    }

    public class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand, UpdateLotResponse>
    {
        private readonly ILotRepository _lotRepository;

        public UpdateLotCommandHandler(ILotRepository lotRepository)
        {
            _lotRepository = lotRepository;
        }

        public Task<UpdateLotResponse> Handle(UpdateLotCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
