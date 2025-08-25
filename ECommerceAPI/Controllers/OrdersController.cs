using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext context;

        public OrdersController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                context.Orders.Add(order);
                context.SaveChanges();
                return Created();
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet] 
        public IActionResult GetOrders()
        {
            var orders = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .ToList();

            var orderDTO = orders.Select(order => new OrderWithOrderItems
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.Name,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ItemTotal = oi.Quantity * oi.UnitPrice
                }).ToList(),
                TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            }).ToList();

            return Ok(orderDTO);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult GetOrderById(int id)
        {
            var order = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var orderDTO = new OrderWithOrderItems
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.Name,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ItemTotal = oi.Quantity * oi.UnitPrice
                }).ToList(),
                TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            };
            return Ok(orderDTO);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateOrder(int id, Order updatedOrder)
        {
            if (id != updatedOrder.Id)
            {
                return BadRequest("Order ID mismatch");
            }
            var existingOrder = context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);
            if (existingOrder == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingOrder.OrderDate = updatedOrder.OrderDate;
                existingOrder.CustomerId = updatedOrder.CustomerId;
                // Update order items
                context.OrderItems.RemoveRange(existingOrder.OrderItems);
                existingOrder.OrderItems = updatedOrder.OrderItems;
                context.SaveChanges();
                return Ok(existingOrder);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOrder(int id)
        {
            var existingOrder = context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);
            if (existingOrder == null)
            {
                return NotFound();
            }
            context.OrderItems.RemoveRange(existingOrder.OrderItems);
            context.Orders.Remove(existingOrder);
            context.SaveChanges();
            return Ok("Order deleted successfully");
        }
    }
}
