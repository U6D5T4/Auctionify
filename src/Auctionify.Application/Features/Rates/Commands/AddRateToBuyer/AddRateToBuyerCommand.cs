using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Rates.Commands.AddRateToBuyer
{
    public class AddRateToBuyerCommand : IRequest<AddRateToBuyerResponse>
    {
        public int LotId { get; set; }

        public string Comment { get; set; }

        public byte RatingValue { get; set; }
    }

    public class AddRateToBuyerCommandHandler
        : IRequestHandler<AddRateToBuyerCommand, AddRateToBuyerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRateRepository _rateRepository;
        private readonly ILotRepository _lotRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public AddRateToBuyerCommandHandler(
            IMapper mapper,
            IRateRepository rateRepository,
            ILotRepository lotRepository,
            ICurrentUserService currentUserService,
            UserManager<User> userManager
        )
        {
            _mapper = mapper;
            _rateRepository = rateRepository;
            _lotRepository = lotRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<AddRateToBuyerResponse> Handle(
            AddRateToBuyerCommand request,
            CancellationToken cancellationToken
        )
        {

            var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

            var lot = await _lotRepository.GetAsync(
                predicate: x => x.Id == request.LotId,
                cancellationToken: cancellationToken
                );

            var rate = new Rate
            {
                SenderId = user.Id,
                ReceiverId = (int)lot.BuyerId,
                LotId = request.LotId,
                Comment = request.Comment,
                RatingValue = request.RatingValue
            };

            var result = _rateRepository.AddAsync(rate);

            var response = _mapper.Map<AddRateToBuyerResponse>(result);

            return response;
        }
    }
}
