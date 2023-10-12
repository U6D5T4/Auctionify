using Auctionify.Application.Features.Lots.Queries.GetAllLots;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Application.Common.Services
{
    public class LotService
    {
        private readonly IMediator mediator;

        public LotService(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<List<GetAllLotsResponse>> GetAllLots()
        {
            return await mediator.Send(new GetAllLotsQuery());
        }
    }
}
