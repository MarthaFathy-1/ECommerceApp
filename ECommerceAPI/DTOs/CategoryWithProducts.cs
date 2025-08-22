namespace ECommerceAPI.DTOs
{
    public class CategoryWithProducts
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> Products { get; set; }
    }
}
