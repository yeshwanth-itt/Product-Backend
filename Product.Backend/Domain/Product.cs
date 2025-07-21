namespace Product.Backend.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public int? Price { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
