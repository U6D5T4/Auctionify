using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Categories.Queries.GetAll
{
	public class GetAllCategoriesQuery : IRequest<IList<GetAllCategoriesResponse>> { }

	public class GetAllCategoriesHandler
		: IRequestHandler<GetAllCategoriesQuery, IList<GetAllCategoriesResponse>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public GetAllCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<IList<GetAllCategoriesResponse>> Handle(
			GetAllCategoriesQuery request,
			CancellationToken cancellationToken
		)
		{
			var result = await _categoryRepository.GetListAsync(
				predicate: c => c.ParentCategoryId == null,
				include: x => x.Include(c => c.Children),
				size: 50
			);

			return _mapper.Map<IList<GetAllCategoriesResponse>>(result.Items);
		}
	}
}
