using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public int? CurrencyId { get; set; }

        public IList<IFormFile>? Photos { get; set; }

        public IList<IFormFile>? AdditionalDocuments {  get; set; }

        public bool IsDraft { get; set; }

    }

    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, CreatedLotResponse>
    {
        private readonly ILotRepository _lotRepository;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CreateLotCommandHandler(ILotRepository lotRepository,
            ILotStatusRepository lotStatusRepository,
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            IMapper mapper)
        { 
            _lotRepository = lotRepository;
            _lotStatusRepository = lotStatusRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CreatedLotResponse> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            AuctionStatus status = request.IsDraft ? AuctionStatus.Draft : AuctionStatus.Upcoming;

            var lotStatus = await _lotStatusRepository.GetAsync(s => s.Name == status.ToString());

            var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail);

            var location = new Location
            {
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
            };

            var lot = new Lot
            {
                SellerId = user.Id,
                Title = request.Title,
                Description = request.Description,
                StartingPrice = request.StartingPrice,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CategoryId = request.CategoryId,
                Location = location,
                CurrencyId = request.CurrencyId,
                LotStatusId = lotStatus.Id
            };

            var createdLot = await _lotRepository.AddAsync(lot);

            return _mapper.Map<CreatedLotResponse>(createdLot);
        }
    }
}
