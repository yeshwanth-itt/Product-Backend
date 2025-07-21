namespace Product.Backend.Application.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

    }
}
