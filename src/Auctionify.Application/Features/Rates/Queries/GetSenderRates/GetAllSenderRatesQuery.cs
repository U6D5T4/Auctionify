using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Rates.Queries.GetSenderRates
{
    public class GetAllSenderRatesQuery : IRequest<GetListResponseDto<GetAllSenderRatesResponse>>
    {
        public PageRequest PageRequest { get; set; }
    }

    public class GetAllSenderRatesQueryHandler
        : IRequestHandler<GetAllSenderRatesQuery, GetListResponseDto<GetAllSenderRatesResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IRateRepository _rateRepository;

        public GetAllSenderRatesQueryHandler(
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

        public async Task<GetListResponseDto<GetAllSenderRatesResponse>> Handle(
            GetAllSenderRatesQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

            var rate = await _rateRepository.GetListAsync(predicate: r => r.RecieverId == user.Id,
            include: x =>
                    x.Include(u => u.Sender),
                enableTracking: false,
                size: request.PageRequest.PageSize,
                index: request.PageRequest.PageIndex,
                cancellationToken: cancellationToken);

            var response = _mapper.Map<GetListResponseDto<GetAllSenderRatesResponse>>(rate);

            return response;
        }
    }
}
