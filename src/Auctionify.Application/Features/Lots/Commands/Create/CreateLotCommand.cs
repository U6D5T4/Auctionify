using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Features.Lots.Commands.Create
{
    public class CreateLotCommand : IRequest<CreatedLotResponse>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }

        public LocationDto Location { get; set; }

        public int? CurrencyId { get; set; }

        public IList<IFormFile>? Photos { get; set; }

        public IList<IFormFile>? AdditionalDocuments {  get; set; }

        public bool IsDraft { get; set; }

    }

    public class LocationDto
    {
        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Address { get; set; }
    }

    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, CreatedLotResponse>
    {
        private readonly ILotRepository lotRepository;
        private readonly IMapper mapper;

        public CreateLotCommandHandler(ILotRepository lotRepository,
            IMapper mapper)
        { 
            this.lotRepository = lotRepository;
            this.mapper = mapper;
        }

        public async Task<CreatedLotResponse> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            var location = new Location
            {
                Address = request.Location.Address,
                City = request.Location.City,
                State = request.Location.State,
                Country = request.Location.Country,
            };

            var lot = new Lot
            {
                Title = request.Title,
                Description = request.Description,
                StartingPrice = request.StartingPrice,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CategoryId = request.CategoryId,
                Location = location,
                CurrencyId = request.CurrencyId,
            };

            var createdLot = await lotRepository.AddAsync(lot);

            return mapper.Map<CreatedLotResponse>(createdLot);
        }
    }
}
