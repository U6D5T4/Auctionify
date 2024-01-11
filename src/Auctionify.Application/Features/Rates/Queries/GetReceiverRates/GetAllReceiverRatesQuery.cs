using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Rates.Queries.GetReceiverRates
{
    public class GetAllReceiverRatesQuery : IRequest<GetListResponseDto<GetAllReceiverRatesResponse>>
    {
        public PageRequest PageRequest { get; set; }
    }

    public class GetAllReceiverRatesQueryHandler
        : IRequestHandler<GetAllReceiverRatesQuery, GetListResponseDto<GetAllReceiverRatesResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IRateRepository _rateRepository;

        public GetAllReceiverRatesQueryHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            IMapper mapper,
            IRateRepository rateRepository

        )
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _rateRepository = rateRepository;
        }
        public async Task<GetListResponseDto<GetAllReceiverRatesResponse>> Handle(GetAllReceiverRatesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

            var feedbacks = await _rateRepository.GetListAsync(predicate: r => r.SenderId == user.Id,
                include: x =>
                    x.Include(u => u.Reciever),
                enableTracking: false,
                size: request.PageRequest.PageSize,
                index: request.PageRequest.PageIndex,
                cancellationToken: cancellationToken);

            var response = _mapper.Map<GetListResponseDto<GetAllReceiverRatesResponse>>(feedbacks);

            return response;
        }
    }
}
