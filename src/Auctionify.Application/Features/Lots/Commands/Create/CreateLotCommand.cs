using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

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

        public IList<IFormFile> Photos { get; set; }

        public IList<IFormFile> AdditionalDocuments {  get; set; }

    }

    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, CreatedLotResponse>
    {
        private readonly ICategoryRepository categoryRepository;

        public CreateLotCommandHandler(ICategoryRepository categoryRepository)
        { 
            this.categoryRepository = categoryRepository;
        }

        public Task<CreatedLotResponse> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
