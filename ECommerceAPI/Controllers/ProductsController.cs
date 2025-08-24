using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProductsController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if(ModelState.IsValid)
            {
                context.Products.AddAsync(product);
                context.SaveChangesAsync();
                return Created();
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = context.Products.Include(p=>p.Category).ToList();
            List<ProductsWithCategory> productDTO = new List<ProductsWithCategory>();
            foreach (var product in products)
            {
                var productsWithCategory = new ProductsWithCategory();
                productsWithCategory.Id = product.Id;
                productsWithCategory.Name = product.Name;
                productsWithCategory.Price = product.Price;
                productsWithCategory.Category = product.Category?.Name;
                productDTO.Add(productsWithCategory);
            }
            return Ok(productDTO);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProductById(int id)
        {
            var existingProduct = context.Products.Include(p => p.Category)
                .Select(c => new ProductsWithCategory
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    Category = c.Category != null ? c.Category.Name : null
                })
                .FirstOrDefault(p => p.Id == id);
            var productsWithCategory = new ProductsWithCategory();

            if (existingProduct != null)
            {
                productsWithCategory.Id = existingProduct.Id;
                productsWithCategory.Name = existingProduct.Name;
                productsWithCategory.Price = existingProduct.Price;
                return Ok(existingProduct);
            }
            return NotFound();
        }

        [HttpPut("{id:int}")]
        public IActionResult EditProduct(int id, Product product)
        {
            var existingProduct = context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    context.SaveChanges();
                    return Ok(product);
                }
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            var existingProduct = context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            context.Remove(existingProduct);
            context.SaveChanges();
            return Ok("Product has been Deleted");
        }
    }
}
