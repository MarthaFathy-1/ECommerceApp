namespace ECommerceAPI.DTOs
{
    public class OrderWithOrderItems
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
