using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly AppDbContext context;

        public OrderItemsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public IActionResult AddOrderItem(OrderItem orderItem)
        {
            if(ModelState.IsValid)
            {
                context.OrderItems.Add(orderItem);
                context.SaveChanges();
                return Created();
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult GetOrderItems()
        {
            var orderItems = context.OrderItems
                .Include(o => o.Product)
                .Select(orderItem => new OrderItemDTO
                {
                    ProductName = orderItem.Product.Name,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    ItemTotal = orderItem.Quantity * orderItem.UnitPrice
                })
                .ToList();

            return Ok(orderItems);
        }


        [HttpGet("{id}")]
        public IActionResult GetOrderItemById(int id)
        {
            var orderItem = context.OrderItems
                .Include(o => o.Product)
                .FirstOrDefault(o => o.Id == id);

            if (orderItem == null)
                return NotFound();

            var dto = new OrderItemDTO
            {
                ProductName = orderItem.Product.Name,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                ItemTotal = orderItem.Quantity * orderItem.UnitPrice
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderItem(int id, OrderItem updatedOrderItem)
        {
            var existingOrderItem = context.OrderItems.Find(id);
            if (existingOrderItem == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                existingOrderItem.Quantity = updatedOrderItem.Quantity;
                existingOrderItem.UnitPrice = updatedOrderItem.UnitPrice;
                existingOrderItem.ProductId = updatedOrderItem.ProductId;
                existingOrderItem.OrderId = updatedOrderItem.OrderId;
                context.SaveChanges();
                return Ok(existingOrderItem);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderItem(int id)
        {
            var existingOrderItem = context.OrderItems.Find(id);
            if (existingOrderItem == null)
                return NotFound();
            context.OrderItems.Remove(existingOrderItem);
            context.SaveChanges();
            return Ok("Order Item removed successfully");
        }

    }
}
