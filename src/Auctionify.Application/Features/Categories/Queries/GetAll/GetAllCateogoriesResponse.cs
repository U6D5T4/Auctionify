﻿using Auctionify.Application.Common.DTOs;

namespace Auctionify.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoriesResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<CategoryDto> Children { get; set; }
    }
}
