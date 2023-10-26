using Auctionify.Application.Common.Interfaces;
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
        private readonly IBlobService _blobService;
        private readonly IFileRepository _fileRepository;

        public UpdateLotCommandHandler(ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			IMapper mapper,
			IBlobService blobService,
			IFileRepository fileRepository)
		{
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_mapper = mapper;
			_blobService = blobService;
			_fileRepository = fileRepository;
		}

		public async Task<UpdateLotResponse> Handle(UpdateLotCommand request, CancellationToken cancellationToken)
        {
            AuctionStatus status = request.IsDraft ? AuctionStatus.Draft : AuctionStatus.Upcoming;

            var lotStatus = await _lotStatusRepository.GetAsync(s => s.Name == status.ToString(), cancellationToken: cancellationToken);

            var lot = await _lotRepository.GetAsync(l => l.Id == request.Id,
                include: x => x.Include(l => l.Location)!,
                cancellationToken: cancellationToken,
                enableTracking: false);

            lot!.LotStatus = lotStatus!;

            var lotUpdated = _mapper.Map(request, lot);

            await _lotRepository.UpdateAsync(lotUpdated);

   //         var folderName = await _fileRepository.GetListAsync(f => f.LotId == lotUpdated.Id, cancellationToken: cancellationToken);
            
   //         var files = new List<File>();
            

   //         if (request.Photos != null)
   //         {
   //             if (folderName.Count == 0)
   //             {
			//		var folder = await _fileRepository.AddAsync(new File { LotId = lotUpdated.Id }, cancellationToken);
			//		folderName.Add(folder);
			//	}
			//}



            return _mapper.Map<UpdateLotResponse>(lotUpdated);
        }
    }
}
