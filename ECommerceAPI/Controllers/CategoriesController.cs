using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext context;

        public CategoriesController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                context.SaveChanges();
                return Created();
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = context.Categories.Include(c=>c.Products).ToList();
            List<CategoryWithProducts> categoryDTO = new List<CategoryWithProducts>();
            foreach (var category in categories)
            {
                var categoryWithProducts = new CategoryWithProducts();
                categoryWithProducts.Id = category.Id;
                categoryWithProducts.Name = category.Name;
                categoryWithProducts.Products = category.Products?.Select(p => p.Name).ToList() ?? new List<string>();
                categoryDTO.Add(categoryWithProducts);
            }
            return Ok(categoryDTO);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult GetCategoryById(int id)
        {
            var existingCategory = context.Categories.Include(c => c.Products)
                .Select(c => new CategoryWithProducts
                {
                    Id = c.Id,
                    Name = c.Name,
                    Products = c.Products != null ? c.Products.Select(p => p.Name).ToList() : new List<string>()
                })
                .FirstOrDefault(c => c.Id == id);
            var categoryWithProducts = new CategoryWithProducts();

            if (existingCategory != null)
            {
                categoryWithProducts.Id = existingCategory.Id;
                categoryWithProducts.Name = existingCategory.Name;
                return Ok(existingCategory);
            }
            return NotFound();
        }

        [HttpPut("{id:int}")]
        [Authorize (Roles = "Admin")]

        public IActionResult EditCategory(int id, Category category)
        {
            var existingCategory = context.Categories.FirstOrDefault(c => c.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    existingCategory.Name = category.Name;
                    context.SaveChanges();
                    return Ok(existingCategory);
                }
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize (Roles = "Admin")]
        public IActionResult DeleteCategory(int id)
        {
            var existingCategory = context.Categories.FirstOrDefault(c => c.Id == id);
            if(existingCategory==null)
            {
                return NotFound();
            }
            context.Remove(existingCategory);
            context.SaveChanges();
            return Ok("Category has been Deleted");
        }

    }
}
