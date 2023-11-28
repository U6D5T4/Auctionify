using Auctionify.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Locations.Queries.GetAll
{
	public class GetAllLocationsQuery : IRequest<IList<GetAllLocationsResponse>>
	{
	}

	public class GetAllLocationsQueryHandler : IRequestHandler<GetAllLocationsQuery, IList<GetAllLocationsResponse>>
	{
		private readonly ILocationRepository _locationRepository;

		public GetAllLocationsQueryHandler(ILocationRepository locationRepository)
		{
			_locationRepository = locationRepository;
		}

		public async Task<IList<GetAllLocationsResponse>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
		{
			var queryable = _locationRepository.Query();

			var listOfCities = await queryable.Select(l =>  l.City )
				.Distinct()
				.Select(city => new GetAllLocationsResponse { City = city })
				.ToListAsync();

			return listOfCities;
		}
	}
}
