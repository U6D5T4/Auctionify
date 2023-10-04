using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.API.Controllers
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }

        public ICollection<Category> Children { get; set; }
    }

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("[action]")]
        public IActionResult CreateCategory()
        {
            var category = new Category
            {
                Name = "Cars"
            };

            var categoryRes = context.Categories.Add(category).Entity;
            context.SaveChanges();

            var result = new CategoryResponse
            {
                Id = categoryRes.Id,
                Name = categoryRes.Name,
                Children = categoryRes.Children,
            };

            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult CreateCategoryChild(string categoryName, int parentCategoryId)
        {
            var category = new Category
            {
                Name = categoryName,
                ParentCategoryId = parentCategoryId
            };

            var categoryRes = context.Categories.Add(category).Entity;

            context.SaveChanges();

            var result = new CategoryResponse
            {
                Id = categoryRes.Id,
                Name = categoryRes.Name,
                Children = categoryRes.Children,
                ParentCategoryId = categoryRes.ParentCategoryId
            };

            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult GetCategoryById(int categoryId)
        {
            var result = context.Categories.Where(c => c.Id == categoryId)
                .ToList();

            context.SaveChanges();

            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult GetParentCategory()
        {
            var result = context.Categories
                .Where(c => c.ParentCategoryId == null)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Children = c.Children,
                    ParentCategoryId = c.ParentCategoryId,
                })
                .ToList();

            context.SaveChanges();

            return Ok(result);
        }
    }
}
