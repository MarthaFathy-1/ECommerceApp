using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
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
            var categories = context.Categories.ToList();
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
        public IActionResult GetCategoryById(int id)
        {
            var existingCategory = context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
            var categoryWithProducts = new CategoryWithProducts();

            if (existingCategory != null)
            {
                categoryWithProducts.Id = existingCategory.Id;
                categoryWithProducts.Name = existingCategory.Name;
                categoryWithProducts.Products = existingCategory.Products?.Select(p => p.Name).ToList();
                return Ok(existingCategory);
            }
            return NotFound();
        }

        [HttpPut("{id:int}")]
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
                    return Ok(category);
                }
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id:int}")]
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
