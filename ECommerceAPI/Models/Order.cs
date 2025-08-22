using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public decimal TotalAmount
        {
            get
            {
                decimal total = 0;
                if (OrderItems != null)
                {
                    foreach (var item in OrderItems)
                    {
                        total += item.Quantity * item.UnitPrice;
                    }
                }
                return total;
            }
        }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
